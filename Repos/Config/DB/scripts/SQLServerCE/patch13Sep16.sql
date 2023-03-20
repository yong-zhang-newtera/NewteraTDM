alter table mm_schema add SUBSCRIBERS ntext null;
create table CM_EVENT_INFO  (
   SCHEMANAME		nvarchar(50)  not null,
   VERSION              nvarchar(50)   not null,
   CLASSNAME            nvarchar(100)   not null,
   EVENTNAME            nvarchar(100)   not null,
   CHECKEDTIME	        datetime
   );
