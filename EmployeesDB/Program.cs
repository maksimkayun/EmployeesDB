using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeesDB.Data.Models;

namespace EmployeesDB
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();

        static void Main(string[] args)
        {
            //Console.WriteLine(GetEmployeesInformation());
            //EmployeesBySalary(Convert.ToInt32(Console.ReadLine()));
            
            /*Console.Write("New address: ");
            var newAddress = Console.ReadLine();
            Console.Write("\nLast name: ");
            var lastName = Console.ReadLine();
            NewAddress(newAddress, lastName);*/
            
            //AuditOfProjects();
            //DossierOnEmployee(int.Parse(Console.ReadLine() ?? string.Empty));
            
            //SmallDepartments(int.Parse(Console.ReadLine() ?? string.Empty));
            //SalaryIncrease(Console.ReadLine(), int.Parse(Console.ReadLine() ?? string.Empty));
            
            //DeleteDepartment(int.Parse(Console.ReadLine() ?? string.Empty));
            DeleteCity(Console.ReadLine());
        }
        
        /// <summary>
        /// Выводит всю информацию о работниках
        /// </summary>
        /// <returns></returns>
        static string GetEmployeesInformation()
        {
            var employees = _context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}");
            }
            return sb.ToString().TrimEnd();
        }
        
        /// <summary>
        /// Выводит полную информацию о сотрудниках, которые получают более
        /// указанной суммы, отсортировав в порядке возрастания по фамилии.
        /// </summary>
        /// <param name="sum"></param>
        static void EmployeesBySalary(int sum) {
            var result = _context.Employees.Where(e => e.Salary > sum)
                .OrderBy(e => e.LastName).Select(e => e);
            int i = 1;
            foreach (var e in result) {
                Console.WriteLine($"{i++}. {e.FirstName} {e.LastName} Salary: {e.Salary}");
            }
        }
        
        /// <summary>
        /// Создает новый адрес и переселяет всех сотрудников с указанной фамилией
        /// по новому адресу
        /// </summary>
        /// <param name="newAddress"></param>
        /// <param name="lastName"></param>
        static void NewAddress(string newAddress, string lastName) {
            var address = new Address {
                AddressText = newAddress
            };
            _context.Addresses.Add(address);
            _context.SaveChanges();

            var addressId = 
                _context.Addresses.SingleOrDefault(e => e.AddressText.Equals(newAddress))?.AddressId;

            var employees = _context.Employees
                .Where(e => e.LastName.Equals(lastName)).Select(e => e).ToList();
            foreach (var v in employees) {
                v.AddressId = addressId;
            }
            _context.SaveChanges();
            
            var res = _context.Employees
                .Where(e => e.LastName.Equals(lastName)).Select(e => e);
            int i = 1;
            foreach (var v in res) {
                Console.WriteLine($"{i}. {v.FirstName} {v.LastName} IDAddress: {v.AddressId}");
            }
        }
        
        /// <summary>
        /// Находит первых 5 работников, у которых были проекты в 2002-2005 годах.
        /// Выводит информацию о них.
        /// </summary>
        static void AuditOfProjects() {
            var projects = _context.Projects
                .Where(e => e.StartDate.Year >= 2002 && e.StartDate.Year <= 2005).Select(e => e).ToList();
            var employees = _context.Employees.ToList();
            foreach (var p in projects) {
                employees = _context.EmployeesProjects
                    .Where(e => e.ProjectId == p.ProjectId).Select(e => e.Employee).Take(5).ToList();
                
            }

            int i = 0;
            foreach (var employee in employees) {
                Console.WriteLine($"Работник: {employee.FirstName} {employee.LastName}. " +
                                  $"Менеджер: {employee.Manager.FirstName} {employee.Manager.LastName}");
                if (projects[i].EndDate != null) {
                    Console.WriteLine($"Проект: {projects[i].Name}. Дата начала: {projects[i].StartDate}." +
                                      $" Дата окончания: {projects[i].EndDate}");
                }
                else {
                    Console.WriteLine($"Проект: {projects[i].Name}. Дата начала: {projects[i].StartDate}." +
                                      $" Дата окончания: НЕ ЗАВЕРШЕН");
                }
                i++;
            }
        }
        
        /// <summary>
        /// Получает ID сотрудника, выводит информацию о нём и названия всех его проектов.
        /// </summary>
        /// <param name="id"></param>
        static void DossierOnEmployee(int id) {
            var employee = _context.Employees.SingleOrDefault(e => e.EmployeeId == id);
            Console.WriteLine($"{employee?.FirstName} {employee?.LastName} - {employee?.JobTitle}");

            var projects = _context.EmployeesProjects.Where(e => e.EmployeeId == employee.EmployeeId)
                .Select(e => e.Project).ToList();
            foreach (var project in projects) {
                Console.WriteLine($"{project.Name}");
            }
        }
        
        /// <summary>
        /// Выводит названия отделов, где менее N сотрудников
        /// </summary>
        static void SmallDepartments(int n) {
            var res = _context.Employees.GroupBy(e => e.DepartmentId)
                .Select(e => new {
                    Count = e.Count(),
                    Key = e.Key
                }).ToList();
            
            foreach (var v in res) {
                if (v.Count < n) {
                    var dept = _context.Departments.SingleOrDefault(e => e.DepartmentId == v.Key)?.Name;
                    Console.WriteLine($"Dep. {dept} - {v.Count} employees");
                }
            }
        }
        
        /// <summary>
        ///  Метод, который увеличивает зарплату отделу department на percent %.
        /// </summary>
        /// <param name="department"></param>
        /// <param name="percent"></param>
        static void SalaryIncrease(string department, int percent) {
            //var deptId = _context.Departments.SingleOrDefault(e => e.Name.Equals(department))?.DepartmentId;
            var employees = _context.Employees.Where(e => e.Department.Name.Equals(department))
                .Select(e => e).ToList();
            foreach (var v in employees) {
                v.Salary += v.Salary * (decimal) (percent / 100.0);
            }
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Метод, удаляющий отдел с заданным Id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException"></exception>
        static void DeleteDepartment(int id) {
            var employees = _context.Employees.Where(e => e.DepartmentId == id)
                .Select(e => e).ToList();
            var dept = _context.Departments.FirstOrDefault(e => e.DepartmentId != id);
            var deptId = dept?.DepartmentId;
            foreach (var emp in employees) {
                if (deptId != null) emp.DepartmentId = (int) deptId;
                emp.ManagerId = dept?.ManagerId;
            }

            var delDept = _context.Departments.SingleOrDefault(e => e.DepartmentId == id);
            _context.Departments.Remove(delDept ?? throw new InvalidOperationException());
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Метод, удаляющий город с заданным названием.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        static void DeleteCity(string name) {
            var town = _context.Towns.SingleOrDefault(e => e.Name.Equals(name));
            int? nullable = null;
            var address = _context.Addresses.SingleOrDefault(e => e.TownId == town.TownId);
            address.TownId = nullable;
            _context.Towns.Remove(town ?? throw new InvalidOperationException());
            _context.SaveChanges();
        }
    }
}
