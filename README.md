<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/654529367/2023.1)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1172357)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Reporting for ASP.NET Core - Row-Level Security 

This example implements connection filtering for reporting applications in multi-user environments. The application sets the current user ID in [SESSION_CONTEXT](https://learn.microsoft.com/en-us/sql/t-sql/functions/session-context-transact-sql?view=sql-server-ver16&viewFallbackFrom=sql-server-ver16). Once the database connection opens, security policies filter visible rows for the current user.

## Configure the Database

1. This example uses a SQL file ([instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)). Execute it to recreate the database locally. Do not forget to update [appsettings.json](./WebReportInterceptors/appsettings.json) so that the connection string works in your environment.

2. Execute the script below. This script extends the database as follows:

- Creates a new schema and predicate function that uses the user ID stored in SESSION_CONTEXT to filter rows. 
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

Use the following script to clean up database resources:

```sql
DROP SECURITY POLICY Security.OrdersFilter;

--DROP TABLE Orders;

DROP FUNCTION Security.fn_securitypredicate;

DROP SCHEMA Security;
```

### Configure the `IDBConnectionInterceptor` Object 
Create an IDBConnectionInterceptor object ([RLSConnectionInterceptor.cs](./WebReportInterceptors/Services/RLSConnectionInterceptor.cs) in this example). When the database connection opens, store the current user ID in SESSION_CONTEXT. Modify queries to the _Orders_ table - filter data by user ID (so as to implement database behavior equivalent to connection filtering). Register `RLSConnectionInterceptor` as an extension in `IServiceCollection`.

### Run the Application

When you run the application, a registration form ([Login.cshtml](./WebReportInterceptors/Views/Account/Login.cshtml)) will appear on-screen. Select a user to generate a report with filtered data.

![Report](./Images/Report.png)

## Files to Review

- [RLSConnectionInterceptor.cs](./WebReportInterceptors/Services/RLSConnectionInterceptor.cs)
- [Program.cs](./WebReportInterceptors/Program.cs)
- [AccountController.cs](./WebReportInterceptors/Controllers/AccountController.cs)
- [Login.cshtml](./WebReportInterceptors/Views/Account/Login.cshtml)

