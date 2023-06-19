<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/654529367/2023.1)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1172357)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Reporting for ASP NET.Core - Implement Row-Level Security 

This example shows how you can implement connection filtering in an application, where users share the same application. The application sets the current user ID in [SESSION_CONTEXT](https://learn.microsoft.com/en-us/sql/t-sql/functions/session-context-transact-sql?view=sql-server-ver16&viewFallbackFrom=sql-server-ver16) after connecting to the database, and then security policies filter rows that shouldn't be visible to this ID.

## Files to Review

- [RLSConnectionInterceptor.cs](./WebReportInterceptors/Services/RLSConnectionInterceptor.cs)
- [Program.cs](./WebReportInterceptors/Program.cs)
- [AccountController.cs](./WebReportInterceptors/Controllers/AccountController.cs)
- [Login.cshtml](./WebReportInterceptors/Views/Account/Login.cshtml)

## Example Overview

### Configure a Database

1. This example uses an SQL file ([instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)). Execute it to recreate a database on your side. Do not forget to update the connection string in the [appsettings.json](./WebReportInterceptors/appsettings.json) file to make it valid in your environment.

2. Execute the script below to configure the execution context to control access to rows in a database table. This script does the following:

- Creates a new schema and predicate function, which uses the user ID stored in SESSION_CONTEXT to filter rows. 
- Creates a security policy that adds this function as a filter predicate and a block predicate on _Orders_.  

```sql
CREATE SCHEMA Security;
GO

CREATE FUNCTION Security.fn_securitypredicate(@EmployeeId int) 
    RETURNS TABLE
    WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS fn_securitypredicate_result
    WHERE CAST(SESSION_CONTEXT(N'EmployeeId') AS int) = @EmployeeId;
GO

CREATE SECURITY POLICY Security.OrdersFilter
    ADD FILTER PREDICATE Security.fn_securitypredicate(EmployeeId)
        ON dbo.Orders,
    ADD BLOCK PREDICATE Security.fn_securitypredicate(EmployeeId)
        ON dbo.Orders AFTER INSERT
    WITH (STATE = ON);
GO
```

### Configure the `IDBConnectionInterceptor` Object 

Create a `IDBConnectionInterceptor` object ([RLSConnectionInterceptor.cs](./WebReportInterceptors/Services/RLSConnectionInterceptor.cs) in this example) to set the current user ID in SESSION_CONTEXT after opening a connection and simulate the connection filtering by selecting from the _Orders_ table after setting different user IDs in SESSION_CONTEXT.

Register `RLSConnectionInterceptor` as an extension in `IServiceCollection`.

### Run the Application

When you run the application, the registration form ([Login.cshtml](./WebReportInterceptors/Views/Account/Login.cshtml)) appears. You can select a user to see the report that displays filtered data for the specified user.

![Report](./Images/Report.png)

## More examples 

[Reporting for WinForms - Override the Default Isolation Level](https://github.com/DevExpress-Examples/winforms-reporting-interceptors)


