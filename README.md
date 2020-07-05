# protoUnitofWorkDapper

prototype to test out unit of work dapper based on Dapper Unit of work site

Objective
- to resolve the transaction scope problem with POCO repository pattern
- solution found in Dapper Unit of work page: https://dapper-tutorial.net/knowledge-base/31298235/how-to-implement-unit-of-work-pattern-with-dapper-
  - by using unit of work pattern to govern the connection that been use in that operation, action
  - for unit of work to control the connection open will result in single transaction scope
  - and the unit of work will be pass into each respository which required to runs in that scope
  
Other
- found another solution to resolve object naming convention standard for server pagination for repository 
 (Ref: https://www.davepaquette.com/archive/2019/01/28/paging-large-result-sets-with-dapper-and-sql-server.aspx)
- class PagedResult<T> is useful to govern naming convention chaos with server pagination
- limitation:
  - exisiting complicated SQL query is still unavoidable, existing implementation: 2 SQL select scripts
   1. get total count with same /**where**/ condition
   2. select with order offser and limit with same /**where**/ conditions
   
 
