create table CM_PATCH (
	patch_name varchar2(50) not null primary key);

create table CM_SUPER_USER (
	NAME varchar2(50) not null,
	PASSWORD varchar2(20) not null,
	ROLE varchar2(15));
insert into CM_SUPER_USER values('admin', '5A191710A4AA7099', 'cm_super_user');

create table CM_ROOT  (
   OID					NUMBER(38)     not null,
   UPDATETIME			DATE		   default SYSDATE,
   constraint PK_CM_ROOT primary key (OID));
   
create table MM_ATTRIBUTE (
	ID NUMBER(38,0) not null,
	NAME VARCHAR2(30) not null,
	DISPLAY_NAME VARCHAR2(100) not null,
	CATEGORY NUMBER(1,0) not null constraint MM_ATTRIBUTECATEGORY_CHK check (CATEGORY in (1,2)) ,
	DESCRIPTION VARCHAR2(2000) null,
	COLUMN_NAME VARCHAR2(30) null,
	CLASS_ID NUMBER(38,0) not null, constraint MM_ATTRIBUTE_PK primary key (ID) ); 

create table MM_RELATION_ATTRIBUTE (
	ATTRIBUTE_ID NUMBER(38,0) not null,
	TYPE NUMBER(1,0) not null constraint MM_RELATION_ATTRIBUTETYPE_CHK check (TYPE in (1,2)) ,
	OWNERSHIP NUMBER(1,0) not null constraint MM_RELATION_OWNERSHIP_CHK check (OWNERSHIP in (1,2,3)) ,
	IS_JOINTABLE NUMBER(1,0) not null,
	JOINTABLE_NAME VARCHAR2(30) null,
	IS_FK NUMBER(1,0) not null,
	REF_CLASS_ID NUMBER(38,0) not null, constraint MM_RELATION_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ); 

create table MM_SIMPLE_ATTRIBUTE (
	ATTRIBUTE_ID NUMBER(38,0) not null,
	TYPE NUMBER(2,0) not null,
	constraint MM_SIMPLE_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ); 

create table MM_PARENT_CHILD (
	PARENT_ID NUMBER(38,0) not null,
	CHILD_ID NUMBER(38,0) not null,
	constraint MM_PARENT_CHILD_PK primary key (PARENT_ID, CHILD_ID) ); 

create table MM_CLASS (
	ID NUMBER(38,0) not null,
	NAME VARCHAR2(30) not null,
	DISPLAY_NAME VARCHAR2(100) not null,
	TABLE_NAME VARCHAR2(30) null,
	SCHEMA_ID NUMBER(38,0) not null,
	PK_NAME varchar2(30),
	constraint MM_CLASS_PK primary key (ID) ); 

create table MM_SCHEMA (
	ID NUMBER(38,0) not null,
	NAME VARCHAR2(50) not null,
	VERSION VARCHAR2(15) not null,
	MAP_METHOD NUMBER(1,0) not null constraint MM_SCHEMAMAP_METHOD_CHK check (MAP_METHOD in (1,2,3)) ,
	XML_SCHEMA CLOB default empty_clob(),
	XACL CLOB default empty_clob(),
	QUERIES CLOB default empty_clob(),
	TAXONOMY CLOB default empty_clob(),
	RULES CLOB default empty_clob(),
	MAPPINGS CLOB default empty_clob(),
	LOG CLOB default empty_clob(),
	constraint MM_SCHEMA_PK primary key (ID) ); 

alter table MM_ATTRIBUTE add constraint MM_ATTRIBUTE_UC1 unique (
	CLASS_ID,
	NAME); 


create unique index CLASS__NAME_SCHEMAID_U on MM_CLASS (
	NAME,
	SCHEMA_ID); 


alter table MM_SCHEMA add constraint MM_SCHEMA_UC1 unique (
	NAME,
	VERSION); 


alter table MM_RELATION_ATTRIBUTE
	add constraint MM_RELATION_ATTRIBUTE_FK1 foreign key (
		ATTRIBUTE_ID)
	 references MM_ATTRIBUTE (
		ID) ON DELETE CASCADE; 

alter table MM_RELATION_ATTRIBUTE
	add constraint MM_RELATION_ATTRIBUTE_REF_FK1 foreign key (
		REF_CLASS_ID)
	 references MM_CLASS (
		ID) ON DELETE CASCADE; 

alter table MM_SIMPLE_ATTRIBUTE
	add constraint MM_SIMPLE_ATTRIBUTE_FK1 foreign key (
		ATTRIBUTE_ID)
	 references MM_ATTRIBUTE (
		ID) ON DELETE CASCADE; 

alter table MM_ATTRIBUTE
	add constraint MM_ATTRIBUTE_CLASS_FK1 foreign key (
		CLASS_ID)
	 references MM_CLASS (
		ID) ON DELETE CASCADE; 

alter table MM_PARENT_CHILD
	add constraint MM_PARENT_CHILD_CLASS_FK1 foreign key (
		PARENT_ID)
	 references MM_CLASS (
		ID) ON DELETE CASCADE; 

alter table MM_PARENT_CHILD
	add constraint MM_PARENT_CHILD_CLASS_FK2 foreign key (
		CHILD_ID)
	 references MM_CLASS (
		ID) ON DELETE CASCADE; 

alter table MM_CLASS
	add constraint MM_CLASS_SCHEMA_FK1 foreign key (
		SCHEMA_ID)
	 references MM_SCHEMA (
		ID) ON DELETE CASCADE; 

create table key_generate(key_value number primary key,
	class_id number not null,
	attribute_id number not null,
	schema_id number not null,	
	transformer_id number not null,
	constraint UN_KEY_CLASS unique(class_id));
insert into key_generate values(0, 0, 1, 1, 1);

create table CM_ATTACHMENT  (
   ID					VARCHAR2(100)  not null ,
   OID					NUMBER(38)     not null,
   NAME                 VARCHAR2(50)   not null,
   CLASSNAME            VARCHAR2(30)   not null,
   TYPE                 VARCHAR2(20)   not null,
   CREATETIME			DATE		   default SYSDATE,
   ASIZE				NUMBER(*,0),
   IS_PUBLIC			NUMBER(1,0)    not null,
   ATTACHMENT			BLOB,
   CONSTRAINT PK_CM_ATTACHMENT PRIMARY KEY (ID),
   CONSTRAINT CM_ATTACHMENT_UN_ID UNIQUE(OID, NAME),
   CONSTRAINT FK_ATTACHEMNT_ROOT FOREIGN KEY(OID)
		REFERENCES CM_ROOT(OID) ON DELETE CASCADE   
   );