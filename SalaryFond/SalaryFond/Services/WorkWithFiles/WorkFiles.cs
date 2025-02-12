﻿using Newtonsoft.Json;
using OfficeOpenXml;
using SalaryFond.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Documents;

namespace SalaryFond.Services.WorkWithFiles
{
    class WorkFiles
    {
        public YearSalary ReadJsonBD(string file_path)
        {
            if (file_path[file_path.Length - 1] == 'n' && file_path[file_path.Length - 2] == 'o' && file_path[file_path.Length - 3] == 's' && file_path[file_path.Length - 4] == 'j')
            {
                YearSalary year = File.Exists(file_path) ? JsonConvert.DeserializeObject<YearSalary>(File.ReadAllText(file_path)) : null;
                return year;
            }
            return null;
        }

        public void WriteJsonBD(YearSalary year, string file_path)
        {
            File.WriteAllText(file_path, JsonConvert.SerializeObject(year));
        }

        public ObservableCollection<YearSalary> ReadJsonBDArchive(string file_path)
        {
            if (file_path[file_path.Length - 1] == 'n' && file_path[file_path.Length - 2] == 'o' && file_path[file_path.Length - 3] == 's' && file_path[file_path.Length - 4] == 'j')
            {
                ObservableCollection<YearSalary> years = File.Exists(file_path) ? JsonConvert.DeserializeObject<ObservableCollection<YearSalary>>(File.ReadAllText(file_path)) : null;

                for (int i = 0; i < years.Count - 1; i++)
                {
                    if (Convert.ToInt32(years[i].Name) < Convert.ToInt32(years[i + 1].Name))
                    {
                        var tmp = years[i];
                        years[i] = years[i + 1];
                        years[i + 1] = tmp;
                    }
                }
                return years;
            }
            return null;
            
        }

        public void WriteJsonBDArchive(ObservableCollection<YearSalary> years, string file_path)
        {
            ObservableCollection<YearSalary> yearsRead = File.Exists(file_path) ? JsonConvert.DeserializeObject<ObservableCollection<YearSalary>>(File.ReadAllText(file_path)) : null;

            if (yearsRead != null)
            {
                List<int> ints = new List<int>();
                for (int i = 0; i < years.Count; i++)
                {
                    for (int j = 0; j < yearsRead.Count; j++)
                    {
                        if (years[i].Name == yearsRead[j].Name)
                        {
                            ints.Add(j);
                        }
                    }

                    yearsRead.Add(years[i]);
                }

                for (int i = 0; i < ints.Count; i++)
                {
                    yearsRead.RemoveAt(ints[i]);
                }

                File.WriteAllText(file_path, JsonConvert.SerializeObject(yearsRead));
            }
            else if (yearsRead == null)
            {
                File.WriteAllText(file_path, JsonConvert.SerializeObject(years));
            }
        }

        public void WriteJsonBDArchiveNew(ObservableCollection<YearSalary> years, string file_path)
        {
            File.WriteAllText(file_path, JsonConvert.SerializeObject(years));
        }

        public void WriteJsonDictionary(ObservableCollection<Company> companies, string file_path)
        {
            ObservableCollection<Company> companiesList = new ObservableCollection<Company>();

            for (int i = 0; i < companies.Count; i++)
            {
                companiesList.Add(new Company()
                {
                    Id = companies[i].Id,
                    Name = companies[i].Name,
                    Location = companies[i].Location,
                    NormalHours = companies[i].NormalHours,
                    PlanningSalaryFund = companies[i].PlanningSalaryFund
                });
            }

            for (int i = 0; i < companies.Count; i++)
            {
                for (int j = 0; j < companies[i].Workers.Count; j++)
                {
                    companiesList[i].Workers.Add(new Worker
                    {
                        Id = companies[i].Workers[j].Id,
                        FIO = companies[i].Workers[j].FIO,
                        MainProfession = companies[i].Workers[j].MainProfession
                });
                }
                
            }

            File.WriteAllText(file_path, JsonConvert.SerializeObject(companiesList));
        }

        public ObservableCollection<Company> ReadJsonDictionary(string file_path)
        {
            if (file_path[file_path.Length - 1] == 'n' && file_path[file_path.Length - 2] == 'o' && file_path[file_path.Length - 3] == 's' && file_path[file_path.Length - 4] == 'j')
            {
                ObservableCollection<Company> companies = File.Exists(file_path) ? JsonConvert.DeserializeObject<ObservableCollection<Company>>(File.ReadAllText(file_path)) : null;
                return companies;
            }
            return null;
        }

        public bool WriteExcel(ObservableCollection<Company> Companies, string file_path)
        {
            var report = new ExcelPackage();

            int countStep = 2;
            int countWorkers = 0;

            for (int i = 1; i < Companies.Count + 1; i++)
            {
                var sheet = report.Workbook.Worksheets.Add(Companies[i - 1].Name);
                sheet.Cells[1, 1, 1, 19].LoadFromArrays(new object[][] { new[] {"№", "ФИО", "Должность", "Оклад",
                "Часы(отработанные)", "Часы(норма)", "руб/час", "Начислено по окладу", "Отпускные", "Больничные", "Премия 10%", 
                    "Премия руководителя", "Штраф", "Итого начислено", "Аванс", "РКО", "Исп.лист", "Перечислено р/с", "Остаток к выдаче"}});
            }

            for (int i = 0; i < report.Workbook.Worksheets.Count; i++)
            {
                countStep = 2;
                for (int j = 0; j < Companies[i].Workers.Count; j++)
                {
                    countWorkers++;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 1].Value = j + 1;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 2].Value = Companies[i].Workers[j].FIO;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 3].Value = Companies[i].Workers[j].MainProfession;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 4].Value = Companies[i].Workers[j].MainSalary;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 5].Value = Companies[i].Workers[j].WorkedHours;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 6].Value = Companies[i].Workers[j].NormalHours;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 7].Value = Companies[i].Workers[j].RateRUB;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 8].Value = Companies[i].Workers[j].MainResultSalary - Companies[i].Workers[j].Prize - Companies[i].Workers[j].HolidayPay - Companies[i].Workers[j].SickPay - Companies[i].Workers[j].PrizeBoss;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 9].Value = Companies[i].Workers[j].HolidayPay;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 10].Value = Companies[i].Workers[j].SickPay;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 11].Value = Companies[i].Workers[j].Prize;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 12].Value = Companies[i].Workers[j].PrizeBoss;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 13].Value = Companies[i].Workers[j].SummPenalties;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 14].Value = Companies[i].Workers[j].FinalResultSalary - Companies[i].Workers[j].SummAdditionalProfessions;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 15].Value = Companies[i].Workers[j].Prepayment;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 16].Value = Companies[i].Workers[j].RKO;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 17].Value = Companies[i].Workers[j].ExecutiveList;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 18].Value = Companies[i].Workers[j].TransferByCard;
                    report.Workbook.Worksheets[i].Cells[j + countStep, 19].Value = Companies[i].Workers[j].ResultSalary - Companies[i].Workers[j].SummAdditionalProfessions;

                    if (Companies[i].Workers[j].AdditionalProfessions.Count > 0)
                    {
                        for (int k = 0; k < Companies[i].Workers[j].AdditionalProfessions.Count; k++)
                        {
                            countWorkers++;
                            countStep++;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 2].Value = Companies[i].Workers[j].FIO;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 3].Value = Companies[i].Workers[j].AdditionalProfessions[k].Name;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 4].Value = Companies[i].Workers[j].AdditionalProfessions[k].MainSalary;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 5].Value = Companies[i].Workers[j].AdditionalProfessions[k].WorkedHours;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 6].Value = Companies[i].Workers[j].AdditionalProfessions[k].NormalHours;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 7].Value = Companies[i].Workers[j].AdditionalProfessions[k].RateRUB;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 8].Value = Companies[i].Workers[j].AdditionalProfessions[k].ResultSalary;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 14].Value = Companies[i].Workers[j].AdditionalProfessions[k].ResultSalary;
                            report.Workbook.Worksheets[i].Cells[j + countStep, 19].Value = Companies[i].Workers[j].AdditionalProfessions[k].ResultSalary;
                        }
                    }
                }

                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 4].Value = "Сумма";
                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 5].Value = Companies[i].WorkedHours;
                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 6].Value = Companies[i].NormalHours;
                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 8].Formula = $"SUM(H2:H{countWorkers + 1})";
                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 14].Formula = $"SUM(N2:N{countWorkers + 1})";
                report.Workbook.Worksheets[i].Cells[countWorkers + 2, 19].Formula = $"SUM(S2:S{countWorkers + 1})";

                report.Workbook.Worksheets[i].Cells[countWorkers + 3, 1].Value = "Плановый фонд";
                report.Workbook.Worksheets[i].Cells[countWorkers + 3, 2].Value = Companies[i].PlanningSalaryFund;

                report.Workbook.Worksheets[i].Cells[countWorkers + 4, 1].Value = "Фактический фонд";
                report.Workbook.Worksheets[i].Cells[countWorkers + 4, 2].Value = Companies[i].FactSalaryFund;

                report.Workbook.Worksheets[i].Cells[countWorkers + 5, 1].Value = "Расхождения";
                report.Workbook.Worksheets[i].Cells[countWorkers + 5, 2].Value = Companies[i].PlanningSalaryFund - Companies[i].FactSalaryFund;
                countWorkers = 0;
            }


            for (int i = 0; i < report.Workbook.Worksheets.Count; i++)
            {
                report.Workbook.Worksheets[i].Cells[1, 1, 1, 19].AutoFitColumns();
                report.Workbook.Worksheets[i].Column(1).Width = 20;
                report.Workbook.Worksheets[i].Column(2).Width = 25;
                report.Workbook.Worksheets[i].Cells[1, 1, 1, 19].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                report.Workbook.Worksheets[i].Cells[1, 1, 1, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            try
            {
                File.WriteAllBytes(file_path, report.GetAsByteArray());
            }
            catch (System.IO.IOException e)
            {
                return false;
            }
            return true;
        }
    }
}