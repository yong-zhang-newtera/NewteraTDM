alter table mm_schema add(SUBSCRIBERS clob default empty_clob());
create table CM_EVENT_INFO  (
   SCHEMANAME		VARCHAR2(50)  not null,
   VERSION              VARCHAR2(50)   not null,
   CLASSNAME            VARCHAR2(100)   not null,
   EVENTNAME            VARCHAR2(100)   not null,
   CHECKEDTIME	        DATE
   );
