using ManagementDashboard_Entities;
using ManagementDashboard_Services;
using ManagementDashboard_Utilites.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;
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

        public async Task<List<Customer>> GetClientListByManagerId(Guid userId, DateTime fromDate, Guid orgId)
        {
            var result = new List<Customer>();
            fromDate = Convert.ToDateTime(fromDate.ToString("MM-dd-yyyy")); //Default get data for 6 month from "From Date"
            var toDate = Convert.ToDateTime(fromDate.AddMonths(6).ToString("MM-dd-yyyy")); //Default get data for 6 month from "From Date"
            var timeSheetsEntries = await base.Find(x => x.Organization == orgId && x.User == userId);// && f.Date >= fromDate && f.Date <= toDate);

            if (timeSheetsEntries != null && timeSheetsEntries.Count > 0)
            {
                result = timeSheetsEntries.ToList().Where(x => x.Date >= fromDate && x.Date <= toDate)
                .Select(s => new Customer()
                {
                    Id = s.Customer,
                    CustomerName = s.CustomerName,
                }).DistinctBy(x => x.Id).ToList();
            }

            return result;
        }

        public async Task<List<TimesheetModel>> GetEmployeeProductivity(Guid managerId, DateTime fromDate, int interval, int mode, string option, Guid customerId)
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
                                      Hours = d.Sum(s => s.Hours) + (d.Sum(s => s.Minutes) / 60),
                                      Minutes = d.Sum(s => s.Minutes) % 60,
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
                                          Hours = d.Sum(s => s.Hours) + (d.Sum(s => s.Minutes) / 60),
                                          Minutes = d.Sum(s => s.Minutes) % 60,
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

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        #region Non_Split_TimeEntries_ByUser
        public async Task<List<TimesheetModel>> GetNonSplitTimesheetEntriesByUser(Guid managerId, DateTime fromDate, int entryCount)
        {
            List<TimesheetModel> lstUserTimesheet = new();
            List<TimesheetModel> productivities = new();
            int entryCounts = 0;
            int daysInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
            DateTime endOfMonth = new(fromDate.Year, fromDate.Month, daysInMonth);
            DateTime toDate = endOfMonth;

            if (entryCount == 0)
            {
                entryCounts = 2;
            }
            else
            {
                entryCounts = entryCount;
            }

            List<UserProfile> employees = new();
            employees = await _userProfileService.GetEmployeesListByManager(managerId);

            foreach (var employee in employees)
            {
                var timesheetEntries = await base.Find(f => f.User == employee.User && f.Date >= fromDate && f.Date <= toDate);
                DateTime dtfromDate = fromDate;

                foreach (DateTime day in EachDay(fromDate, toDate))
                {
                    DateTime dtToDate = day.AddHours(23).AddMinutes(59);
                    int getDay = day.Day;

                    var productindex = (from p in timesheetEntries.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate)
                                        group p by new
                                        {
                                            date = day,
                                            week = getDay,
                                            month = p.Date.Month,
                                            userId = p.User,
                                        } into d
                                        select new TimesheetModel()
                                        {
                                            Date = d.Key.date,
                                            MonthYrPart = string.Format("{0}/{1}", d.Key.week, d.Key.month),
                                            UserId = d.Key.userId,
                                            UserCode = employee.UserCode,
                                            UserName = employee.FirstName + " " + employee.LastName,
                                            Hours = d.Sum(s => s.Hours) + (d.Sum(s => s.Minutes) / 60),
                                            Minutes = d.Sum(s => s.Minutes) % 60,
                                            Count = d.Select(s => s.User).Count(),
                                            TimeOffHours = d.Where(x => x.Project == new Guid(AppConstants.TimeOffProject)).Sum(s => s.Hours),
                                            //NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
                                        }).ToList();

                    var nonSplitUsers = productindex.Where(x => x.Count <= entryCount);

                    productivities.AddRange(nonSplitUsers);
                    dtfromDate = day.AddDays(1);
                }
            }

            lstUserTimesheet = productivities.ToList().GroupBy(x => new { x.UserName, x.UserCode, x.UserId }).Select(x => new TimesheetModel()
            {
                UserId = x.Key.UserId,
                UserName = x.Key.UserName,
                UserCode = x.Key.UserCode,
                Count = x.Select(s => s.UserName).Count(),
                Hours = (int)(x.Sum(s => s.Hours) + x.Sum(s => s.Minutes) / 60),
                Minutes = (int)(x.Sum(s => s.Minutes) % 60),
                NonProductivity = x.Sum(s => s.NonProductivity),
                TimeOffHours = x.Sum(s => s.TimeOffHours),
            }).ToList();

            foreach (var item in lstUserTimesheet)
            {
                int days = 0;
                int monthpart = fromDate.Month;
                int yearpart = fromDate.Year;

                days = Enumerable.Range(1, DateTime.DaysInMonth(yearpart, monthpart))
                    .Select(day => new DateTime(yearpart, monthpart, day))
                    .Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
                                 d.DayOfWeek != DayOfWeek.Sunday);
                item.Hours = item.Hours - item.TimeOffHours; // Minus the Hours and  Time Off Hours from the collection
                int NoofWorkingHr = days * 8;
                int sam = (Convert.ToInt32(item.TimeOffHours) / 8);
                item.Count = item.Count - (Convert.ToInt32(item.TimeOffHours) / 8);// Minus the Count from Time Off Hours and divided by 8
                                                                                   //int ActualWorkingHr = NoofWorkingHr - HolidayHr;
                int ActualWorkingHr = NoofWorkingHr - Convert.ToInt32(item.TimeOffHours);  // Minus the No of Working hours & Holiday hours & TimeOffHours

                decimal ProductivityHr = item.Hours - Convert.ToInt32(item.TimeOffHours);// - NonProductiviry; // Minus the Hours and Non Productivity from the Collection

                decimal percentage = 0;
                if (ActualWorkingHr > 0)
                    percentage = (ProductivityHr / ActualWorkingHr) * 100;

                item.Percentage = Math.Round(percentage).ToString();
            }
            lstUserTimesheet = lstUserTimesheet.Where(l => l.Count > 0).OrderByDescending(x => x.Count).ThenBy(u => u.UserName).ToList();

            return lstUserTimesheet;
        }
        #endregion

        #region Non_Split_TimeEntries_ByUser_ByWeekly
        public async Task<List<NonSplitEntry>> GetNonSplitTimesheetEntriesByUserByWeekly(Guid managerId, DateTime fromDate, int entryCount, Guid userId)
        {
            List<TimesheetModel> lstUserTimesheet = new();
            List<NonSplitEntry> lstNonSplitEntries = new();

            int daysInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
            DateTime endOfMonth = new(fromDate.Year, fromDate.Month, daysInMonth);
            DateTime toDate = endOfMonth;
            int entryCounts = 0;

            if (entryCount == 0)
            {
                entryCounts = 2;
            }
            else
            {
                entryCounts = entryCount;
            }

           UserProfile employee = await _userProfileService.GetEmployeeById(userId);
            //employees = await _userProfileService.GetEmployeesListByManager(managerId);

            if (employee != null)
            {
                var timesheetEntries = await base.Find(f => f.User == userId && f.Date >= fromDate && f.Date <= toDate);
                DateTime dtfromDate = fromDate;

                foreach (DateTime day in EachDay(fromDate, toDate))
                {
                    DateTime dtToDate = day.AddHours(23).AddMinutes(59);
                    int getDay = day.Day;

                    var productindex = (from p in timesheetEntries.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate)
                                        group p by new
                                        {
                                            date = day,
                                            week = getDay,
                                            month = p.Date.Month,
                                            userId = p.User,
                                        } into d
                                        select new TimesheetModel()
                                        {
                                            Date = d.Key.date,
                                            MonthYrPart = string.Format("{0}/{1}", d.Key.week, d.Key.month),
                                            UserId = d.Key.userId,
                                            UserCode = employee.UserCode,
                                            UserName = employee.FirstName + " " + employee.LastName,
                                            Hours = d.Sum(s => s.Hours) + (d.Sum(s => s.Minutes) / 60),
                                            Minutes = d.Sum(s => s.Minutes) % 60,
                                            Count = d.Select(s => s.User).Count(),
                                            TimeOffHours = d.Where(x => x.Project == new Guid(AppConstants.TimeOffProject)).Sum(s => s.Hours),
                                            //NonProductivity = d.Where(x => x.IsProductivity == false).Sum(s => s.Hours),
                                        }).ToList();

                    var nonSplitUsers = productindex.Where(x => x.Count <= entryCount);

                    lstUserTimesheet.AddRange(nonSplitUsers);
                    dtfromDate = day.AddDays(1);
                }
            }
            

            if (lstUserTimesheet.Count > 0)
            {
                DateTime dtfromDate = fromDate;
                foreach (DateTime day in EachWeek(fromDate, toDate))
                {
                    DateTime dtToDate = day.AddDays(6);
                    NonSplitEntry productivitiesindex = new();
                    List<NonSplitDrillDonw> data = new List<NonSplitDrillDonw>();
                    int getDay = day.Day;

                    productivitiesindex = (from p in lstUserTimesheet.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate)
                                           group p by new
                                           {
                                               week = getDay,
                                               month = p.Date.Month,
                                               year = p.Date.Year,
                                               userId = p.UserId,
                                               userCode = p.UserCode,
                                               userName = p.UserName
                                           } into d
                                           select new NonSplitEntry()
                                           {
                                               WeeklyStart = string.Format("{0}/{1}/{2}", d.Key.week, d.Key.month, d.Key.year),
                                               UserId = d.Key.userId,
                                               UserCode = d.Key.userCode,
                                               UserName = d.Key.userName,
                                               Hours = d.Sum(s => s.Hours),
                                               Count = d.Count(),
                                               TimeOffHours = d.Sum(s => s.TimeOffHours),
                                               NonProductivity = d.Sum(x => x.NonProductivity),
                                           }).OrderByDescending(g => g.WeeklyStart).FirstOrDefault();

                    data = (from p in lstUserTimesheet.ToList().Where(x => x.Date >= dtfromDate && x.Date <= dtToDate && x.TimeOffHours <= 0)
                            group p by new
                            {
                                date = p.Date,
                                userId = p.UserId,
                                userCode = p.UserCode,
                                userName = p.UserName
                            } into d
                            select new NonSplitDrillDonw()
                            {
                                Date = string.Format("{0}", d.Key.date),
                                Count = d.Count(),
                            }).OrderByDescending(g => g.Date).ToList();
                    if (productivitiesindex != null)
                    {
                        productivitiesindex.nonSplitDrillDonws = data;
                        lstNonSplitEntries.Add(productivitiesindex);
                        dtfromDate = day.AddDays(7);
                    }
                }
            }

            foreach (var item in lstNonSplitEntries)
            {
                int days = 0;
                int monthpart = fromDate.Month;
                int yearpart = fromDate.Year;

                days = Enumerable.Range(1, DateTime.DaysInMonth(yearpart, monthpart))
                    .Select(day => new DateTime(yearpart, monthpart, day))
                    .Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
                                 d.DayOfWeek != DayOfWeek.Sunday);
                item.Hours = item.Hours - item.TimeOffHours; // Minus the Hours and  Time Off Hours from the collection
                int NoofWorkingHr = days * 8;

                item.Count = item.Count - (Convert.ToInt32(item.TimeOffHours) / 8);// Minus the Count from Time Off Hours and divided by 8
                                                                                   //int ActualWorkingHr = NoofWorkingHr - HolidayHr;
                int ActualWorkingHr = NoofWorkingHr - Convert.ToInt32(item.TimeOffHours);//NoofWorkingHr - HolidayHr - Convert.ToInt32(item.TimeOffHours);  // Minus the No of Working hours & Holiday hours & TimeOffHours

                decimal ProductivityHr = item.Hours - Convert.ToInt32(item.TimeOffHours);// - NonProductiviry; // Minus the Hours and Non Productivity from the Collection

                decimal percentage = 0;
                if (ActualWorkingHr > 0)
                    percentage = (ProductivityHr / ActualWorkingHr) * 100;

                item.Percentage = Math.Round(percentage).ToString();
            }
            lstNonSplitEntries = lstNonSplitEntries.Where(l => l.Count > 0).OrderByDescending(x => x.Count).ThenBy(u => u.UserName).ToList();

            return lstNonSplitEntries;
            }
            #endregion
        }
    }