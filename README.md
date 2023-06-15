# SQL Data Migration

This solution is designed to migrate your data from any server to any destination. In one shot you can transfer data from multiple servers to multiple destinations. However, it must use same login credentials both sides as there is only one login.

Below json shows a config that is saved in Migration.Config.json file. This should be configured. 

```
[
  {
    "SourceServer": "server1",
    "SourceDb": "db1",
    "TargetServer": "server2",
    "TargetDb": "db2",
    "DbContextId": "1"
  }
]
```
Other thing to notice is that you could create DbContext manually or you could use the following code to generate database context from actual DB. This process is called <b>Scaffolding (Reverse Engineering)</b>

```
Scaffold-DbContext "{connection string}"
```

Enter above command in Package Manager Console and make sure default project is set to the project that you want to generate entities. For information to know about this command, click [here](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#dotnet-ef-dbcontext-scaffold).

After that, generated dbconext needs to be changed like it is shown in sample.

```
public partial class SampleDbContext : BaseContext<SampleDbContext>
```

After that, generate migration script my executing following command in package manager console
```
Add-Migration {name}
```

Once the migration is generated, run the console app. It will ask to provide username & password to log into db server. After that, console app will check the TargetDb exists at destination server. If exists, app will ask whether to reset db or not. 

If migration is successful, you will have nice carbon copy of your source db at the destination. However, if one enitity is failed, you could go to target db and check tables and verification queries under migration schema. If you wish to add new set of queries to verfiy data migration, you could add by overriding following method in your DbContext.

```
public override void InsertData(ModelBuilder modelBuilder, string connection)
```

Enjoy
