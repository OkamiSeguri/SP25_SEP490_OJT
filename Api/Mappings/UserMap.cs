using BusinessObject;
using CsvHelper.Configuration;
using FOMSOData.Models;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Map(u => u.MSSV).Name("MSSV");
        Map(u => u.FullName).Name("FullName");
        Map(u => u.Email).Name("Email");
        Map(u => u.Password).Name("Password");
    }
}
