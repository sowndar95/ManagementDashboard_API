using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementDashboard_Entities
{
    public sealed class CustomTime
    {
            public int? Hours { get; set; }
            public int? Minutes { get; set; }

            /// <summary>
            /// Used only in Reports, to ignore Nulls and Zeros
            /// </summary>
            public string FormattedValueIgnoredNulls
            {
                get
                {
                    if (Hours.HasValue == false && Minutes.HasValue == false)
                    {
                        return "";
                    }

                    string hours = "";
                    if (Hours.HasValue && Hours.Value != 0)
                    {
                        hours = Hours.Value.ToString("D2");
                    }

                    string minutes = "";
                    if (Minutes.HasValue && Minutes.Value != 0)
                    {
                        minutes = Minutes.Value.ToString("D2");
                    }

                    if (string.IsNullOrEmpty(hours) && string.IsNullOrEmpty(minutes))
                    {
                        return "";
                    }

                    return ConvertTimeToDisplayFormat(Hours, Minutes);
                }
            }

            /// <summary>
            /// Used everywhere except Reports, with the formatted value.
            /// </summary>
            public string FormattedValue
            {
                get
                {
                    return ConvertTimeToDisplayFormat(Hours, Minutes);
                }
            }

            public static string ConvertTimeToDisplayFormat(int? arghours, int? argMinutes)
            {
                int hours = arghours.HasValue ? arghours.Value : 0;
                int Minutes = argMinutes.HasValue ? argMinutes.Value : 0;

                string value = string.Empty;
                string minutes = string.Empty;
                switch (Minutes)
                {
                    case 0:
                        minutes = "00";
                        break;
                    case 15:
                        minutes = "25";
                        break;
                    case 30:
                        minutes = "50";
                        break;
                    case 45:
                        minutes = "75";
                        break;
                    default:
                        throw new Exception("Incorrect Minutes Value");
                }
                value = $"{hours.ToString("D2")}.{minutes}";
                return value;
            }

        }
}
