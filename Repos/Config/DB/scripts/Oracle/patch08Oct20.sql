create table CM_PIVOT_LAYOUTS (
	ID VARCHAR2(100)  not null,
	SCHEMANAME VARCHAR2(50),
	VERSION VARCHAR2(10),
	CLASSNAME VARCHAR2(50)  not null,
	NAME VARCHAR2(50) not null,
	VIEWNAME VARCHAR2(50),
	DESCRIPTION VARCHAR2(200),
        CREATETIME DATE default SYSDATE,
	XML CLOB default empty_clob(),
	CONSTRAINT CM_PIVOT_LAYOUTS_PK PRIMARY KEY (ID));

create table WF_TASK_SUBSTITUTE (
	ID VARCHAR2(10)  not null,
	XML clob default empty_clob()
	);

insert into WF_TASK_SUBSTITUTE(ID, XML) values('1', empty_clob());