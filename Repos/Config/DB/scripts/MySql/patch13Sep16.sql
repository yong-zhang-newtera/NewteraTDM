alter table mm_schema add(SUBSCRIBERS longtext);

create table CM_EVENT_INFO  (
   SCHEMANAME		VARCHAR(50)  not null,
   VERSION              VARCHAR(50)   not null,
   CLASSNAME            VARCHAR(100)   not null,
   EVENTNAME            VARCHAR(100)   not null,
   CHECKEDTIME	        DATETIME
   ) ENGINE=InnoDB;