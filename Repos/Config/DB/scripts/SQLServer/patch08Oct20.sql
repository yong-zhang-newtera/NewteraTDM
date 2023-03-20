create table CM_PIVOT_LAYOUTS (
	ID nvarchar(100)  not null,
	SCHEMANAME nvarchar(50),
	VERSION nvarchar(10),
	CLASSNAME nvarchar(50) not null,
	NAME nvarchar(50) not null,
	VIEWNAME nvarchar(50),
	DESCRIPTION nvarchar(200),
        CREATETIME datetime default getdate(),
	XML text,
	CONSTRAINT CM_PIVOT_LAYOUTS_PK PRIMARY KEY (ID)); 

create table WF_TASK_SUBSTITUTE (
	ID nvarchar(10)  not null,
	XML text null
);

insert into WF_TASK_SUBSTITUTE(ID, XML) values('1', null);