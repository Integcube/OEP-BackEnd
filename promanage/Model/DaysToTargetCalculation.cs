using Microsoft.AspNetCore.Diagnostics;
using System;

namespace ActionTrakingSystem.Model
{
    public static class DaysToTargetCalculation
    {
        public static string CalculateMonths2(DateTime endDate, int statusId, string budgetCalc, string implementationCalc, string partsCalc, string sapCalc, string evidenceCalc)
        {
            try
            {
                DateTime startDate = DateTime.Now;
                var TotalDays = endDate.Subtract(startDate).Days;

                if (statusId == 3)
                {
                    //if (budgetCalc != string.Empty && implementationCalc != string.Empty && partsCalc != string.Empty && sapCalc != string.Empty && evidenceCalc != string.Empty)
                    //{
                    //    var budgetV = convertDecimal(budgetCalc);
                    //    var implementationCalcV = convertDecimal(implementationCalc);
                    //    var partsCalcV = convertDecimal(partsCalc);
                    //    var sapCalcV = convertDecimal(sapCalc);
                    //    var evidenceCalcV = convertDecimal(evidenceCalc);
                    //    if (budgetV == 0 && implementationCalcV==0 && partsCalcV==0 && sapCalcV==0 && evidenceCalcV==0)
                    //    {
                    //        if (TotalDays <= 7)
                    //            return "Less than a week";
                    //        else if (TotalDays <= 30)
                    //            return "Less than a Month";
                    //        else if (TotalDays <= 180)
                    //            return "Less than 6 Month";
                    //        else
                    //            return "NotDue";
                    //    }
                    //    return "Closed";
                    //}
                    //else
                        return "Closed";

                }
                else
                {
                    if (startDate > endDate)
                        return "OverDue";
                    else
                    {
                        if (TotalDays <= 7)
                            return "Less than a week";
                        else if (TotalDays <= 30)
                            return "Less than a Month";
                        else if (TotalDays <= 180)
                            return "Less than 6 Month";
                        //else if (TotalDays < 365)
                        //    return "Greate than 6 Month";
                        else
                            return "NotDue";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;

            }

        }
        public static decimal convertDecimal(string input)
        {
            var val = input.Replace("%", "");
            decimal decimalValue = decimal.Parse(val);
            return decimalValue;
        }
        public static string CalculateMonths(DateTime? endDate, int? statusId, decimal? calcDate, decimal? calcStatus, decimal? calcEvid)
        {
            if ((calcEvid == 1 && calcStatus == 1) || statusId == 1)
            {
                //if (calcDate != 1 && calcEvid != 1 && calcStatus != 1)
                //{
                //    return "NotDue";
                //}
                return "Closed";
            }
               else if (statusId == 4)
            {
                return "Implemented";
            }
            else
            {
                DateTime startDate = DateTime.Now;
                if (startDate > endDate)
                    return "OverDue";
                else
                {
                    var TotalDays = endDate?.Subtract(startDate).Days;
                    if (TotalDays <= 7)
                        return "Less than a week";
                    else if (TotalDays <= 30)
                        return "Less than a Month";
                    else if (TotalDays <= 180)
                        return "Less than 6 Month";
                    else if (TotalDays < 365)
                        return "Greate than 6 Month";
                    else
                        return "NotDue";
                }
            }
        }

        public static int CalculateStatusId(decimal? finalImpScore, bool isCompleted, bool rework)
        {
            if (rework == true)
            {
                return 2;

            }
            else if (finalImpScore == 0)
            {
                return 2;

            }
            else if (finalImpScore == 1 && isCompleted == true)
            {
                return 3;
            }
            else if(finalImpScore == 1 && isCompleted == false)
            {
                return 5;
            }
            else
            {
                return 4;
            }
        }
        public static string CalculateStatusTitle(decimal? finalImpScore, bool isCompleted, bool rework)
        {
            if(rework == true)
            {
                return "Open";
            }
            else if (finalImpScore == 0)
            {
                return "Open";
            }
            else if (finalImpScore == 1 && isCompleted == true)
            {
                return "Closed";
            }
            else if (finalImpScore == 1 && isCompleted == false)
            {
                return "Implemented";
            }
            else
            {
                return "In Progress";
            }
        }

        public static string NewCalcTil(decimal? finalImpScore, DateTime endDate, bool isCompleted, bool rework)
        {
            try
            {
                DateTime startDate = DateTime.Now;
                var TotalDays = endDate.Subtract(startDate).Days;
                //if (rework == true)
                //{
                //    return "Open";
                //}
                 if (finalImpScore == 1 && isCompleted == true)
                {
                    //else
                    return "Closed";

                }
                else if (finalImpScore == 1 && isCompleted == false)
                {
                    return "Implemented";

                }
                else if (startDate > endDate)
                {
                    return "OverDue";

                }
                else if (TotalDays <= 7)
                {
                    return "Less than a week";

                }
                else if (TotalDays <= 30)
                {
                    return "Less than a Month";

                }
                else if (TotalDays <= 180)
                {
                    return "Less than 6 Month";

                }
                else
                    return "NotDue";
                //else
                //{
                //    if (startDate > endDate)
                //        return "OverDue";
                //    else
                //    {
                //        if (TotalDays <= 7)
                //            return "Less than a week";
                //        else if (TotalDays <= 30)
                //            return "Less than a Month";
                //        else if (TotalDays <= 180)
                //            return "Less than 6 Month";
                //        //else if (TotalDays < 365)
                //        //    return "Greate than 6 Month";
                //        else
                //            return "NotDue";
                //    }
                //}
            }
            catch (Exception e)
            {
                return e.Message;

            }
        }
    }
}

