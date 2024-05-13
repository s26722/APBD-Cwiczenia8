using LinqTasks.Extensions;
using LinqTasks.Models;

namespace LinqTasks;

public static partial class Tasks
{
    public static IEnumerable<Emp> Emps { get; set; }
    public static IEnumerable<Dept> Depts { get; set; }

    static Tasks()
    {
        Depts = LoadDepts();
        Emps = LoadEmps();
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Job = "Backend programmer";
    /// </summary>
    public static IEnumerable<Emp> Task1()
    {
        return Emps.Where(emp => emp.Job == "Backend programmer");
    }

    /// <summary>
    ///     SELECT * FROM Emps Job = "Frontend programmer" AND Salary>1000 ORDER BY Ename DESC;
    /// </summary>
    public static IEnumerable<Emp> Task2()
    {
        return Emps.Where(emp => emp.Job == "Frontend programmer" && emp.Salary > 1000)
            .OrderByDescending(emp => emp.Ename);
    }


    /// <summary>
    ///     SELECT MAX(Salary) FROM Emps;
    /// </summary>
    public static int Task3()
    {
        return Emps.Max(emp => emp.Salary);
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Salary=(SELECT MAX(Salary) FROM Emps);
    /// </summary>
    public static IEnumerable<Emp> Task4()
    {
        int maxSalary = Task3();
        return Emps.Where(emp => emp.Salary == maxSalary);
    }

    /// <summary>
    ///    SELECT ename AS Nazwisko, job AS Praca FROM Emps;
    /// </summary>
    public static IEnumerable<object> Task5()
    {
        return Emps.Select(emp => new { Nazwisko = emp.Ename, Praca = emp.Job });
    }

    /// <summary>
    ///     SELECT Emps.Ename, Emps.Job, Depts.Dname FROM Emps
    ///     INNER JOIN Depts ON Emps.Deptno=Depts.Deptno
    ///     Rezultat: Złączenie kolekcji Emps i Depts.
    /// </summary>
    public static IEnumerable<object> Task6()
    {
        return Emps.Join(Depts, emp => emp.Deptno, dept => dept.Deptno,
            (emp, dept) => new { emp.Ename, emp.Job, dept.Dname });
    }

    /// <summary>
    ///     SELECT Job AS Praca, COUNT(1) LiczbaPracownikow FROM Emps GROUP BY Job;
    /// </summary>
    public static IEnumerable<object> Task7()
    {
        return Emps.GroupBy(emp => emp.Job)
            .Select(group => new { Praca = group.Key, LiczbaPracownikow = group.Count() });
    }

    /// <summary>
    ///     Zwróć wartość "true" jeśli choć jeden
    ///     z elementów kolekcji pracuje jako "Backend programmer".
    /// </summary>
    public static bool Task8()
    { 
        return Emps.Any(emp => emp.Job == "Backend programmer");
    }

    /// <summary>
    ///     SELECT TOP 1 * FROM Emp WHERE Job="Frontend programmer"
    ///     ORDER BY HireDate DESC;
    /// </summary>
    public static Emp Task9()
    {
        return Emps.Where(emp => emp.Job == "Frontend programmer")
            .OrderByDescending(emp => emp.HireDate)
            .FirstOrDefault();
    }

    /// <summary>
    ///     SELECT Ename, Job, Hiredate FROM Emps
    ///     UNION
    ///     SELECT "Brak wartości", null, null;
    /// </summary>
    public static IEnumerable<object> Task10()
    {
        var empData = Emps.Select(emp => new { emp.Ename, emp.Job, emp.HireDate });
        var defaultData = new[] { new { Ename = "Brak wartości", Job = (string)null, HireDate = (DateTime?)null } };
        return empData.Union(defaultData);
    }

    /// <summary>
    ///     Wykorzystując LINQ pobierz pracowników podzielony na departamenty pamiętając, że:
    ///     1. Interesują nas tylko departamenty z liczbą pracowników powyżej 1
    ///     2. Chcemy zwrócić listę obiektów o następującej srukturze:
    ///     [
    ///     {name: "RESEARCH", numOfEmployees: 3},
    ///     {name: "SALES", numOfEmployees: 5},
    ///     ...
    ///     ]
    ///     3. Wykorzystaj typy anonimowe
    /// </summary>
    public static IEnumerable<object> Task11()
    {
        IEnumerable<object> result = null;
        result = (from emp in Emps join dept in Depts on emp.Deptno equals dept.Deptno 
            group dept by dept.Dname into g where g.Count() > 1 select new { name = g.Key, numOfEmployees = g.Count()}).ToList();
        return result;
    }

    /// <summary>
    ///     Napisz własną metodę rozszerzeń, która pozwoli skompilować się poniższemu fragmentowi kodu.
    ///     Metodę dodaj do klasy CustomExtensionMethods, która zdefiniowana jest poniżej.
    ///     Metoda powinna zwrócić tylko tych pracowników, którzy mają min. 1 bezpośredniego podwładnego.
    ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
    /// </summary>
    public static IEnumerable<Emp> Task12()
    {
        return Emps.GetEmpsWithSubordinates().OrderBy(emp => emp.Ename).ThenByDescending(emp => emp.Salary);
    }

    /// <summary>
    ///     Poniższa metoda powinna zwracać pojedyczną liczbę int.
    ///     Na wejściu przyjmujemy listę liczb całkowitych.
    ///     Spróbuj z pomocą LINQ'a odnaleźć tę liczbę, które występuja w tablicy int'ów nieparzystą liczbę razy.
    ///     Zakładamy, że zawsze będzie jedna taka liczba.
    ///     Np: {1,1,1,1,1,1,10,1,1,1,1} => 10
    /// </summary>
    public static int Task13(int[] arr)
    {
        return arr.GroupBy(x => x)
            .Where(g => g.Count() % 2 != 0)
            .Select(g => g.Key)
            .Single();
    }

    /// <summary>
    ///     Zwróć tylko te departamenty, które mają 5 pracowników lub nie mają pracowników w ogóle.
    ///     Posortuj rezultat po nazwie departament rosnąco.
    /// </summary>
    public static IEnumerable<Dept> Task14()
    {
        return Depts.Where(dept => Emps.Count(emp => emp.Deptno == dept.Deptno) == 5 || 
                                   !Emps.Any(emp => emp.Deptno == dept.Deptno))
            .OrderBy(dept => dept.Dname);
       
    }
}