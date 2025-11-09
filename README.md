# EmployeeService — Test Task (ADO.NET, WCF REST)

## Project Description
This project implements two REST API endpoints using **ADO.NET** (without Entity Framework) and **WCF**.
It demonstrates working with relational data, building a hierarchical employee tree using a recursive SQL CTE,
and updating employee status via HTTP requests.

Implemented methods:

1. `GET /GetEmployeeById?id={id}` — returns an employee hierarchy in a tree structure.
2. `PUT /EnableEmployee?id={id}&enable={true|false}` — updates the `Enable` flag for a specific employee.

---

## Setup and Run Instructions

### 1. Create and Populate the Database

Run the following SQL script in your **SQL Server** instance:

```sql
CREATE TABLE Employee
(
    ID INT PRIMARY KEY,
    Name VARCHAR(100),
    ManagerID INT NULL,
    Enable BIT
);

INSERT INTO Employee (ID, Name, ManagerID, Enable) VALUES
(1, 'Andrey', NULL, 1),
(2, 'Alexey', 1, 1),
(3, 'Roman', 1, 1),
(4, 'Oleg', 2, 1),
(5, 'Irina', 2, 1),
(6, 'Elena', 3, 1);
```

---

### 2. Configure the Connection String

Open the **`Web.config`** file and set your own connection string inside the `<connectionStrings>` section:

```xml
<connectionStrings>
  <add name="EmployeeDb"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=YOUR_DATABASE_NAME;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> ⚠️ Replace `YOUR_SERVER_NAME` and `YOUR_DATABASE_NAME` with your actual SQL Server details.

---

### 3. Run the Project

1. Open the solution in **Visual Studio**.
2. Make sure the startup project is **WCF Service Application**.
3. Press **F5** or **Ctrl+F5** to launch.
   The service will be available at a URL like:

   ```
   http://localhost:64014/EmployeeService/EmployeeService.svc
   ```

   *(Your port may differ depending on Visual Studio settings.)*

---

## API Testing Examples

You can test the service via **browser**, **Postman**, or **curl**.
Replace `PORT` with your local port number.

### 1️⃣ Get Employee Tree

```bash
GET http://localhost:PORT/EmployeeService/EmployeeService.svc/GetEmployeeById?id=1
```

**Example JSON Response:**

```json
{
  "ID": 1,
  "Name": "Andrey",
  "Enable": true,
  "Subordinates": [
    {
      "ID": 2,
      "Name": "Alexey",
      "Enable": true,
      "Subordinates": [
        { "ID": 4, "Name": "Oleg", "Enable": true, "Subordinates": [] },
        { "ID": 5, "Name": "Irina", "Enable": true, "Subordinates": [] }
      ]
    },
    {
      "ID": 3,
      "Name": "Roman",
      "Enable": true,
      "Subordinates": [
        { "ID": 6, "Name": "Elena", "Enable": true, "Subordinates": [] }
      ]
    }
  ]
}
```

---

### 2️⃣ Enable or Disable an Employee

```bash
PUT http://localhost:PORT/EmployeeService/EmployeeService.svc/EnableEmployee?id=3&enable=false
```

To re-enable the same employee:

```bash
PUT http://localhost:PORT/EmployeeService/EmployeeService.svc/EnableEmployee?id=3&enable=true
```

**Possible HTTP Responses:**
200 OK                  Employee updated successfully
404 Not Found           Employee ID not found in database
500 Internal Server     Error Unexpected server error

---

## Example URLs (replace port with yours)

```text
http://localhost:64014/EmployeeService/EmployeeService.svc/GetEmployeeById?id=1
http://localhost:64014/EmployeeService/EmployeeService.svc/EnableEmployee?id=3&enable=false
```

---

## Author
**Tkachenko Oleksandr**
Test task for a **.NET Backend Developer** position.