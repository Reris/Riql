# Riql

**Riql** (Reduced Items Query Language) is a small straight forward query parser for all Linq queryables like Entity Framework, Linq2Db, InMemory-Lists, etc.

It helps you and your consumers to use filter conditions like where, orderby, skip, take, and even reduced content simply by applying a Riql-string to a Queryable.

The main goal is writing short and simple REST-APIs without all the specification noise.

## Quick Start
1. Add the **Riql** NuGet-package to your project.
2. Modify Startup class in your project 
```csharp
// This method could be a REST API, a DataContext or any other data accessor.
// In this case, 'Entities' ist just an plain InMemory-List, while a EF-DbSet would already be a queryable.
public IEnumerable<Entity> GetEntities(string riql)
{
    return this.Entities.AsQueryable().ApplyRiql(riql);
}
```	

3. Access your Data via Riql-request strings:

```csharp
[...]
var entities = api.GetEntities("$where=Active==true $orderby=Price desc")
[...]

```

### Further Examples
* `$where=FirstName==John`
* `$where=FirstName==Jo*`
* `$where=Price>=100;Price<1000 $orderby=Price desc $take=20 $skip=100`

## Supported Frameworks
* [x] .NET Standard 2.0+


# Query Language
While I was searching for an easier way to create queries using REST without all the noise of defining controller actions just for queries and not full blown CRUD,
I've digged my way through OData, GraphQL etc until I stumbled over [RSQL](https://github.com/jirutka/rsql-parser).
But I needed it for .Net and it would be improved a lot if it doesnt only support filtering.

The language is divided in 2 parts: The Riql superset for and the Rsql subset.

## W.O.R.S.T
Is the Riql superset, it defines the query parts:
1. **W**here: A filter query via Rsql. `$where=Active==true`
2. **O**rderby: A list of properties to order by `$orderby=Active,Price desc`
3. **R**educe: A list of properties to be reduce the result to, if some of the other properties would require lots of data. `$reduce=Id,Price`
4. **S**kip: Skip the first N items, like for pagination. `$skip=100`
5. **T**ake: Take the next N items, like for pagination. `$take=20`

Good to know: 
* There is no need to write the full keyword. Only the first letter counts too. `$w=Active==true$o=Price`
* You can define the more then once and the order then counts. Like an second orderby *after* a skip.

## Rsql
Rsql is the filtering subset.

### Grouping order
As in any language, the priviledged order counts:
1. group: `(`...`)`
2. and: `;`
3. or: `,`

Example: Get all high priced *or* high priority which are active: `$where=(Price>=100,HighPriority==true);Active==true`

### Comparison operators
To compare values:
* `==` / `=eq=`: Equals. `$where=Active==true`
* `!=` / `=neq=`: Not equals. `$where=RoomNr!=0`
* `<` / `=lt=`: Less than. `$where=Price<100`
* `<=` / `=le=`: Less than equal. `$where=Price<=100`
* `>` / `=gt=`: Greater than. `$where=Price>100`
* `>=` / `=ge=`: Greater than equal. `$where=Price>=100`
* `=nil=` / `=is-null=`: Check if null. `$where=AssignedTo=nil=true`
* `=in=`: Check if the value is any of these. `$where=RoomNr=in=(101,105)`
* `=nin=` / `=out=`: Check if the value is not any of these. `$where=RoomNr=nin=(101,105)`

Good to know: 
* You need a whitespaced string? Use single or double quotes `$w=Name=='John Doe'` / `$w=Name=="John Doe"`
* You need wildcards? Ask the stars `$w=Name==Jo*` / `$w=Name==*Do*`
* You need escape sequences? Use a backslashes `$w=Story=='Don\'t \*'`


### Security
While Riql doesn't affect any security as it just reduces the data delivered anyways, it does not lift you from the burden to restrict the amount of published data.

Inspired by [Autumn.MVC](https://github.com/gwendallg/autumn.mvc) and [RSQL / FIQL parser](https://github.com/jirutka/rsql-parser)