using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using ManagementDashboard_Utilites.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManagementDashboard_Services
{
    public sealed class TimeSheetService : BaseService<TimeSheet>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserProfileService _userProfileService;
        public TimeSheetService(ApplicationSettings settings, UserManager<ApplicationUser> userManager, UserProfileService userProfileService) : base(settings)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
        }

        public async Task<List<TimesheetModel>> GetEmployeeTimesheetEntries(Guid managerId, DateTime fromDate, int interval, int mode, string option, Guid customerId)
        {
            List<UserProfile> employees = new();
            List<TimeSheet> timesheets = new();
            List<TimesheetModel> lstUserTimesheet = new();
            DateTime toDate;
            int totalInterval = (interval * 7) - 1;
            if (mode == 0) // 0: Weekly, 1:Monthly
            {
                toDate = fromDate.AddDays(totalInterval);
            }
            else
            {
                DateTime Interval = fromDate.AddMonths(--interval);
                int daysInMonth = DateTime.DaysInMonth(Interval.Year, Interval.Month);
                DateTime endOfMonth = new DateTime(Interval.Year, Interval.Month, daysInMonth);
                toDate = endOfMonth;
            }

            if (customerId != Guid.Empty)
            {
                employees = await _userProfileService.GetEmployeesListByCustomer(managerId, customerId);
            }
            else
            {
                employees = await _userProfileService.GetEmployeesListByManager(managerId);
            }

            foreach (var employee in employees)
            {
                if (mode == 1)
                {
                    var timesheetEntries = await base.Find(f => f.User == employee.User && f.Date >= fromDate && f.Date <= toDate);
                    var result = (from p in timesheetEntries.ToList()
                                  group p by new
                                  {
                                      month = p.Date.Month,
                                      year = p.Date.Year,
                                      userCode = p.User,
                                  } into d
                                  select new TimesheetModel()
                                  {
                                      MonthYrPart = string.Format("{0}/{1}", d.Key.month, d.Key.year),
                                      UserId = d.Key.userCode,
                                      UserCode = employee.UserCode,
                                      UserName = employee.FirstName + " " + employee.LastName,
                                      Hours = d.Sum(s => s.Hours), //include mins
                                      Count = d.Count(),
                                      TimeOffHours = d.Where(x => x.Project == new Guid(AppConstants.TimeOffProject)).Sum(s => s.Hours),
                                      //NonProductivity = d.Where(x => x. == false).Sum(s => s.Hours),
                                  }).OrderByDescending(g => g.MonthYrPart).ToList();
                    lstUserTimesheet.AddRange(result);
                }
                else
                {
                    DateTime dtfromDate = fromDate;

                    foreach (DateTime day in EachWeek(fromDate, toDate))
                    {
                        DateTime dtToDate = day.AddDays(6).AddHours(23).AddMinutes(59);
                        int getDay = day.Day;
                        int getMonth = dtfromDate.Month;

                        var timesheetEntries = await base.Find(f => f.User == employee.User && f.Date >= dtfromDate && f.Date <= dtToDate);

                        var result = (from p in timesheetEntries.ToList()
                                      group p by new
                                      {
                                          week = getDay,
                                          month = getMonth,
                                          userCode = p.User,
                                      } into d
                                      select new TimesheetModel()
                                      {
                                          WeeklyStart = string.Format("{0}/{1}", d.Key.week, d.Key.month),
                                          UserCode = employee.UserCode,
                                          UserName = employee.FirstName + " " + employee.LastName,
                                          Hours = d.Sum(s => s.Hours),
                                          Count = d.Count(),
                                          TimeOffHours = d.Where(x => x.Project == new Guid(AppConstants.TimeOffProject)).Sum(s => s.Hours),
                                          //NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
                                      }).OrderByDescending(g => g.WeeklyStart).ToList();

                        lstUserTimesheet.AddRange(result);
                        dtfromDate = day.AddDays(7);
                    }
                }
            }

            foreach (var item in lstUserTimesheet)
            {
                int NoOfHoliday = 0;
                int days = 0;

                if (mode == 1)
                {
                    string[] dtyrPart = item.MonthYrPart.Split('/');
                    int monthpart = Convert.ToInt16(dtyrPart[0]);
                    int yearpart = Convert.ToInt16(dtyrPart[1]);
                    days = Enumerable.Range(1, DateTime.DaysInMonth(yearpart, monthpart))
                    .Select(day => new DateTime(yearpart, monthpart, day))
                    .Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
                                 d.DayOfWeek != DayOfWeek.Friday); //Get Actual Working from the month exculded Saturday and Sunday
                }
                else
                {
                    string[] dtweekPart = item.WeeklyStart.Split('/');
                    int daypart = Convert.ToInt16(dtweekPart[0]);
                    int monthpart = Convert.ToInt16(dtweekPart[1]);
                    int yearpart = lstUserTimesheet.FirstOrDefault().Date.Year;
                    days = 5; // Consider only working days. Removed DayOfWeek.Saturday & DayOfWeek.Sunday
                }              

                item.Hours = item.Hours - item.TimeOffHours; // Minus the Hours and  Time Off Hours from the collection
                int NoofWorkingHr = days * 8; // 5 * 8 = 40 ==> Total days multiply with 8 hours 
                                              //int HolidayHr = NoOfHoliday * 8;  // 0 * 8 = 0 ==> No Of Holiday multiply with 8 hours

                int ActualWorkingHr = NoofWorkingHr - Convert.ToInt32(item.TimeOffHours); //NoofWorkingHr - HolidayHr - Convert.ToInt32(item.TimeOffHours);  // Minus the No of Working hours & Holiday hours & TimeOffHours

                item.WorkingDays = days - (Convert.ToInt32(item.TimeOffHours) / 8); //days - NoOfHoliday - (Convert.ToInt32(item.TimeOffHours) / 8);
                item.Count = item.Count - (Convert.ToInt32(item.TimeOffHours) / 8);// Minus the Count from Time Off Hours and divided by 8

                decimal NonProductiviry = item.NonProductivity - item.TimeOffHours; // Get Non Productivity from the Collection
                decimal ProductivityHr = item.Hours - NonProductiviry; // Minus the Hours and Non Productivity from the Collection
                item.NonProductivity = NonProductiviry;
                item.ProductivityHours = ProductivityHr;
                decimal percentage = 0;
                if (ActualWorkingHr > 0)
                    percentage = (ProductivityHr / ActualWorkingHr) * 100; // Dividing Productivity Hours / Actual Working Hours * Multiply 100
                item.Percentage = Math.Round(percentage).ToString();
            }

            return lstUserTimesheet;
        }

        public IEnumerable<DateTime> EachWeek(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(7))
                yield return day;
        }

        //public List<Productivity> EmployeeProductivity(ProjectTimeEntry model)
        //{

        //    UserTimesheetDetailsModel userTimesheet = new UserTimesheetDetailsModel();
        //    List<UserTimesheetInHours> list = new List<UserTimesheetInHours>();
        //    List<UserTimesheetInHours> lstUserTimesheet = new List<UserTimesheetInHours>();
        //    List<Productivity> productivities = new List<Productivity>();

        //    userTimesheet.userID = model.userID;
        //    userTimesheet.OrganizationId = model.OrganizationId;
        //    userTimesheet.Fromdate = model.Fromdate;
        //    userTimesheet.Option = model.Option;
        //    userTimesheet.ProjectIds = model.ProjectId > 1 ? model.ProjectId.ToString() : string.Empty;
        //    int totalInterval = (model.Interval * 7) - 1;
        //    if (model.Mode == 0) // 0: Weekly, 1:Monthly
        //    {
        //        userTimesheet.ToDate = userTimesheet.Fromdate.AddDays(totalInterval);
        //    }
        //    else
        //    {
        //        DateTime interval = userTimesheet.Fromdate.AddMonths(--model.Interval);
        //        int daysInMonth = DateTime.DaysInMonth(interval.Year, interval.Month);
        //        DateTime endOfMonth = new DateTime(interval.Year, interval.Month, daysInMonth);
        //        userTimesheet.ToDate = endOfMonth;
        //    }

        //    //List<HolidayMgmtApp> lstHoliday = new List<HolidayMgmtApp>();
        //    //lstHoliday = GetHolidays().Where(x => x.IsWeekEnd == false).ToList();

        //    lstUserTimesheet = GetUserTimesheetDetailsByManager(userTimesheet, null);
        //    if (model.ProjectId > 0)
        //    {
        //        lstUserTimesheet = lstUserTimesheet.Where(l => l.ProjectId == model.ProjectId).ToList();
        //    }
        //    if (model.Mode == 0) // 0: Weekly, 1:Monthly
        //    {
        //        // Weekly
        //        productivities = GetWeeklyProductivity(lstUserTimesheet, userTimesheet, totalInterval);
        //    }
        //    else // Monthly
        //    {
        //        if (userTimesheet.Option.ToLower() == "employee")
        //        {
        //            productivities = (from p in lstUserTimesheet.ToList()
        //                              group p by new
        //                              {
        //                                  month = p.Date.Month,
        //                                  year = p.Date.Year,
        //                                  userCode = p.UserCode,
        //                                  userName = p.UserName
        //                              } into d
        //                              select new Productivity()
        //                              {
        //                                  MonthYrPart = string.Format("{0}/{1}", d.Key.month, d.Key.year),
        //                                  UserCode = d.Key.userCode,
        //                                  UserName = d.Key.userName,
        //                                  Hours = d.Sum(s => s.Hours),
        //                                  Count = d.Count(),
        //                                  TimeOffHours = d.Where(x => x.ProjectId == 8).Sum(s => s.Hours),
        //                                  NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
        //                              }).OrderByDescending(g => g.MonthYrPart).ToList();

        //        }
        //        else if (userTimesheet.Option.ToLower() == "customer")
        //        {
        //            productivities = (from p in lstUserTimesheet.ToList()
        //                              group p by new
        //                              {
        //                                  month = p.Date.Month,
        //                                  year = p.Date.Year,
        //                                  userCode = p.UserCode,
        //                                  userName = p.UserName

        //                              } into d
        //                              select new Productivity()
        //                              {
        //                                  MonthYrPart = string.Format("{0}/{1}", d.Key.month, d.Key.year),
        //                                  UserCode = d.Key.userCode,
        //                                  UserName = d.Key.userName,
        //                                  Hours = d.Sum(s => s.Hours),
        //                                  Count = d.Count(),
        //                                  TimeOffHours = d.Where(x => x.ProjectId == 8).Sum(s => s.Hours),
        //                                  NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
        //                              }).OrderByDescending(g => g.MonthYrPart).ToList();
        //        }

        //        list = lstUserTimesheet.ToList().GroupBy(x => new { x.Organization, x.Project, x.ProjectTask, x.Date }).Select(x => new UserTimesheetInHours()
        //        {
        //            Organization = x.Key.Organization,
        //            Project = x.Key.Project,
        //            ProjectTask = x.Key.ProjectTask,
        //            Hours = x.Sum(s => s.Hours),
        //            Date = x.Key.Date,
        //        }).ToList();


        //        foreach (var item in productivities)
        //        {
        //            int NoOfHoliday = 0;
        //            int days = 0;

        //            string[] dtyrPart = item.MonthYrPart.Split('/');
        //            int monthpart = Convert.ToInt16(dtyrPart[0]);
        //            int yearpart = Convert.ToInt16(dtyrPart[1]);

        //            //NoOfHoliday = lstHoliday.Where(x => x.HolidayDate.Year == yearpart && x.HolidayDate.Month == monthpart).Count();

        //            days = Enumerable.Range(1, DateTime.DaysInMonth(yearpart, monthpart))
        //                .Select(day => new DateTime(yearpart, monthpart, day))
        //                .Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
        //                             d.DayOfWeek != DayOfWeek.Friday); //Get Actual Working from the month exculded Saturday and Sunday

        //            item.Hours = item.Hours - item.TimeOffHours; // Minus the Hours and  Time Off Hours from the collection
        //            int NoofWorkingHr = days * 8; // 5 * 8 = 40 ==> Total days multiply with 8 hours 
        //                                          //int HolidayHr = NoOfHoliday * 8;  // 0 * 8 = 0 ==> No Of Holiday multiply with 8 hours

        //            int ActualWorkingHr = NoofWorkingHr - Convert.ToInt32(item.TimeOffHours); //NoofWorkingHr - HolidayHr - Convert.ToInt32(item.TimeOffHours);  // Minus the No of Working hours & Holiday hours & TimeOffHours

        //            item.WorkingDays = days - (Convert.ToInt32(item.TimeOffHours) / 8); //days - NoOfHoliday - (Convert.ToInt32(item.TimeOffHours) / 8);
        //            item.Count = item.Count - (Convert.ToInt32(item.TimeOffHours) / 8);// Minus the Count from Time Off Hours and divided by 8

        //            decimal NonProductiviry = item.NonProductivity - item.TimeOffHours; // Get Non Productivity from the Collection
        //            decimal ProductivityHr = item.Hours - NonProductiviry; // Minus the Hours and Non Productivity from the Collection
        //            item.NonProductivity = NonProductiviry;
        //            item.ProductivityHours = ProductivityHr;
        //            decimal percentage = 0;
        //            if (ActualWorkingHr > 0)
        //                percentage = (ProductivityHr / ActualWorkingHr) * 100; // Dividing Productivity Hours / Actual Working Hours * Multiply 100

        //            item.Percentage = Math.Round(percentage).ToString();
        //        }
        //    }
        //    return productivities;
        //}



        //public List<Productivity> GetWeeklyProductivity(List<UserTimesheetInHours> lstUserTimesheet, UserTimesheetDetailsModel userTimesheet, int interval)
        //{
        //    DateTime fromDate = userTimesheet.Fromdate;
        //    DateTime dtfromDate = userTimesheet.Fromdate;
        //    DateTime toDate = fromDate.AddDays(interval);
        //    List<Productivity> productivities = new List<Productivity>();
        //    if (userTimesheet.Option.ToLower() == "employee")
        //    {
        //        foreach (DateTime day in EachWeek(fromDate, toDate))
        //        {
        //            DateTime dtToDate = day.AddDays(6).AddHours(23).AddMinutes(59);
        //            List<Productivity> productivitiesindex = new List<Productivity>();
        //            int getDay = day.Day;
        //            int getMonth = dtfromDate.Month;

        //            productivitiesindex = (from p in lstUserTimesheet.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate)
        //                                   group p by new
        //                                   {
        //                                       week = getDay,
        //                                       month = getMonth,
        //                                       userCode = p.UserCode,
        //                                       userName = p.UserName
        //                                   } into d
        //                                   select new Productivity()
        //                                   {
        //                                       WeeklyStart = string.Format("{0}/{1}", d.Key.week, d.Key.month),
        //                                       UserCode = d.Key.userCode,
        //                                       UserName = d.Key.userName,
        //                                       Hours = d.Sum(s => s.Hours),
        //                                       Count = d.Count(),
        //                                       TimeOffHours = d.Where(x => x.ProjectId == 8).Sum(s => s.Hours), // ProjectTaskId=14 is holiday & handle separatly using holiday Calender
        //                                       NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
        //                                   }).OrderByDescending(g => g.WeeklyStart).ToList();

        //            productivities.AddRange(productivitiesindex);
        //            dtfromDate = day.AddDays(7);
        //        }
        //    }
        //    else if (userTimesheet.Option.ToLower() == "customer")
        //    {
        //        foreach (DateTime day in EachWeek(fromDate, toDate))
        //        {
        //            DateTime dtToDate = day.AddDays(6);
        //            List<Productivity> productivitiesindex = new List<Productivity>();
        //            int getDay = day.Day;
        //            productivitiesindex = (from p in lstUserTimesheet.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate)
        //                                   group p by new
        //                                   {
        //                                       week = getDay,
        //                                       month = p.Date.Month,
        //                                       userCode = p.UserCode,
        //                                       userName = p.UserName
        //                                   } into d
        //                                   select new Productivity()
        //                                   {
        //                                       WeeklyStart = string.Format("{0}/{1}", d.Key.week, d.Key.month),
        //                                       UserCode = d.Key.userCode,
        //                                       UserName = d.Key.userName,
        //                                       Hours = d.Sum(s => s.Hours),
        //                                       Count = d.Count(),
        //                                       NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
        //                                   }).OrderByDescending(g => g.WeeklyStart).ToList();

        //            productivities.AddRange(productivitiesindex);
        //            dtfromDate = day.AddDays(7);
        //        }

        //    }
        //    try
        //    {
        //        foreach (var item in productivities)
        //        {
        //            int NoOfHoliday = 0;
        //            int days = 0;
        //            string[] dtweekPart = item.WeeklyStart.Split('/');
        //            int daypart = Convert.ToInt16(dtweekPart[0]);
        //            int monthpart = Convert.ToInt16(dtweekPart[1]);
        //            int yearpart = lstUserTimesheet.FirstOrDefault().Date.Year;

        //            //DateTime dtFromHolidayDate = Convert.ToDateTime(yearpart + "/" + monthpart + "/" + daypart);
        //            //DateTime dtToHolidayDate = dtFromHolidayDate.AddDays(6);
        //            //foreach (DateTime day in EachWeek(dtFromHolidayDate, dtToHolidayDate))
        //            //{
        //            //    NoOfHoliday = GetHolidaysByFromToDate(dtFromHolidayDate, dtToHolidayDate); //lstHoliday.Where(x => x.HolidayDate.Year == yearWeekpart && x.HolidayDate.Month == monthWeekpart && x.HolidayDate.Day == dayWeekpart).Count();
        //            //}
        //            days = 5; // Consider only working days. Removed DayOfWeek.Saturday & DayOfWeek.Sunday

        //            item.Hours = item.Hours - item.TimeOffHours; // Minus the Hours and  Time Off Hours from the collection
        //            int NoofWorkingHr = days * 8; // 5 * 8 = 40 ==> Total days multiply with 8 hours 
        //            int HolidayHr = NoOfHoliday * 8;  // 0 * 8 = 0 ==> No Of Holiday multiply with 8 hours

        //            item.Count = item.Count - (Convert.ToInt32(item.TimeOffHours) / 8);// Minus the Count from Time Off Hours and divided by 8
        //            int ActualWorkingHr = NoofWorkingHr - Convert.ToInt32(item.TimeOffHours); //NoofWorkingHr - HolidayHr - Convert.ToInt32(item.TimeOffHours);  // Minus the No of Working hours & Holiday hours & TimeOffHours

        //            item.WorkingDays = days - (Convert.ToInt32(item.TimeOffHours) / 8);//days - NoOfHoliday - (Convert.ToInt32(item.TimeOffHours) / 8);
        //            decimal NonProductiviry = item.NonProductivity - item.TimeOffHours;
        //            decimal ProductivityHr = item.Hours - NonProductiviry;  //  Minus the hours and Non Productiviry hours
        //            item.NonProductivity = NonProductiviry;
        //            item.ProductivityHours = ProductivityHr;
        //            decimal percentage = 0;
        //            if (ActualWorkingHr > 0)
        //                percentage = (ProductivityHr / ActualWorkingHr) * 100;  // Divided Productivity hours and ActualWorking hours and multiply by 100 to get the percentage

        //            item.Percentage = Math.Round(percentage).ToString();

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return productivities;
        //}

    }
}


