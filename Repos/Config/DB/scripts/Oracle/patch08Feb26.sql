create table CM_CHART_TEMP_INFO (
	ID VARCHAR2(100)  not null,
	SCHEMANAME VARCHAR2(50),
	VERSION VARCHAR2(10),
	CLASSNAME VARCHAR2(50)  not null,
	NAME VARCHAR2(50) not null,
	TYPE VARCHAR2(15) not null,
        DESCRIPTION VARCHAR2(200),
	CREATETIME DATE default SYSDATE,
	XML CLOB default empty_clob(),
	CONSTRAINT CM_CHART_TEMP_INFO_PK PRIMARY KEY (ID)); 