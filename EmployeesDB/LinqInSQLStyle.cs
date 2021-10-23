using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeesDB.Data.Models;

namespace EmployeesDB
{
    public class LinqInSQLStyle
    {
        private static EmployeesContext _context = new EmployeesContext();
        
        /// <summary>
        /// Начало выполнения команд LINQ в стиле SQL
        /// </summary>
        public static void Start() {
            //Console.WriteLine(GetEmployeesInformation());
            //EmployeesBySalary(int.Parse(Console.ReadLine() ?? string.Empty));
            
            /*Console.Write("New address: ");
            var newAddress = Console.ReadLine();
            Console.Write("\nLast name: ");
            var lastName = Console.ReadLine();
            NewAddress(newAddress, lastName);*/
            
            //AuditOfProjects();
            //DossierOnEmployee(int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
            //SmallDepartments(int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
            
            //SalaryIncrease(Console.ReadLine(), int.Parse(Console.ReadLine() ?? string.Empty));
            //DeleteDepartment(int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
            DeleteCity(Console.ReadLine());
        }
        
        /// <summary>
        /// Выводит всю информацию о работниках
        /// </summary>
        /// <returns></returns>
        static string GetEmployeesInformation() {
            var employees = (from e in _context.Employees
                orderby e.EmployeeId
                select new {
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.JobTitle
                }).ToList();
            var output = new StringBuilder();
            foreach (var e in employees) {
                output.AppendLine($"{e.FirstName} {e.MiddleName} {e.LastName} {e.JobTitle}");
            }

            return output.ToString().TrimEnd();
        }
        
        /// <summary>
        /// Выводит полную информацию о сотрудниках, которые получают более
        /// указанной суммы, отсортировав в порядке возрастания по фамилии.
        /// </summary>
        /// <param name="sum"></param>
        static void EmployeesBySalary(int sum) {
            var result = from e in _context.Employees
                orderby e.LastName
                where e.Salary > sum
                select e;
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

            var addressId = (from ad in _context.Addresses
                where ad.AddressText.Equals(newAddress)
                select ad.AddressId)?.SingleOrDefault();

            var employees = (from e in _context.Employees 
                where e.LastName.Equals(lastName) select e).ToList();
            
            foreach (var v in employees) {
                v.AddressId = addressId;
            }
            _context.SaveChanges();

            var res = from e in _context.Employees where e.LastName.Equals(lastName) select e;
            int i = 1;
            foreach (var v in res) {
                Console.WriteLine($"{i++}. {v.FirstName} {v.LastName} IDAddress: {v.AddressId}");
            }
        }
        
        /// <summary>
        /// Находит первых 5 работников, у которых были проекты в 2002-2005 годах.
        /// Выводит информацию о них.
        /// </summary>
        static void AuditOfProjects() {
            var projects = (from p in _context.Projects 
                where p.StartDate.Year >= 2002 && p.StartDate.Year <= 2005 select p).ToList();

            List<Employee> employees = _context.Employees.ToList();
            foreach (var p in projects) {
                employees = (from emp in _context.EmployeesProjects
                    where emp.ProjectId == p.ProjectId
                    select emp.Employee).Take(5).ToList();
            }
            
            int i = 0;
            foreach (var employee in employees) {
                Console.WriteLine($"{i + 1}. Работник: {employee.FirstName} {employee.LastName}. " +
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
            var employee = (from e in _context.Employees where e.EmployeeId == id select e).SingleOrDefault();
            Console.WriteLine($"{employee?.FirstName} {employee?.LastName} - {employee?.JobTitle}");

            var projects = (from p in _context.EmployeesProjects
                where p.EmployeeId == employee.EmployeeId
                select p.Project.Name).ToList();
            
            foreach (var project in projects) {
                Console.WriteLine($"{project}");
            }
        }
        
        /// <summary>
        /// Выводит названия отделов, где менее N сотрудников
        /// </summary>
        static void SmallDepartments(int n) {
            var res = (from employee in _context.Employees
                group employee by employee.DepartmentId
                into employeeGroup
                select new {
                    Count = employeeGroup.Count(),
                    Key = employeeGroup.Key,
                }).ToList();
            
            foreach (var v in res) {
                if (v.Count < n) {
                    var dept = (from d in _context.Departments where d.DepartmentId == v.Key select d)
                        .SingleOrDefault()?.Name;
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
            var employees = (from e in _context.Employees
                where e.Department.Name.Equals(department)
                select e).ToList();
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
            // var employees = _context.Employees.Where(e => e.DepartmentId == id)
            //     .Select(e => e).ToList();
            var employees = 
                (from e in _context.Employees where e.DepartmentId == id select e).ToList();
            
            var dept = 
                (from d in _context.Departments where d.DepartmentId != id select d).FirstOrDefault();
            
            var deptId = dept?.DepartmentId;
            foreach (var emp in employees) {
                if (deptId != null) emp.DepartmentId = (int) deptId;
                emp.ManagerId = dept?.ManagerId;
            }

            var delDept = 
                (from d in _context.Departments where d.DepartmentId == id select d).SingleOrDefault();
            _context.Departments.Remove(delDept ?? throw new InvalidOperationException());
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Метод, удаляющий город с заданным названием.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        static void DeleteCity(string name) {
            var town = (from t in _context.Towns where t.Name.Equals(name) select t).SingleOrDefault();
            
            var addresses = 
                (from ad in _context.Addresses where ad.TownId == town.TownId select ad).ToList();
            foreach (var address in addresses) {
                address.TownId = null;
            }
           
            _context.Towns.Remove(town ?? throw new InvalidOperationException());
            _context.SaveChanges();
        }
    }
}