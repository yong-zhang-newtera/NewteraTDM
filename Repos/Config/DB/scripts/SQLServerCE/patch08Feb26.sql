create table CM_CHART_TEMP_INFO (
	ID nvarchar(100)  not null,
	SCHEMANAME nvarchar(50),
	VERSION nvarchar(10),
	CLASSNAME nvarchar(50) not null,
	NAME nvarchar(50) not null,
	TYPE nvarchar(15) not null,
	DESCRIPTION nvarchar(200),
        CREATETIME datetime default getdate(),
	XML ntext,
	CONSTRAINT CM_CHART_TEMP_INFO_PK PRIMARY KEY (ID)); 