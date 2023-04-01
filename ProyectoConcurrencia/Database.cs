

namespace Proyecto.Validaciones.DTOS;

public static class Database
{

    public static readonly List<Guid> id = new();
    public static readonly List<SalesDataTransferObject> sales = new();
    public static readonly List<BranchesDataTransferObject> branches = new();
    public static readonly List<EmployeesDataTransferObject> employees = new();
    public static readonly List<CarsDataTransferObject> cars = new();
    public static readonly List<string> errors = new();
}


