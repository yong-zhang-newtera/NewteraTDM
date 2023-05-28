create table CM_PATCH (
	patch_name varchar(50) not null primary key);

create table CM_SUPER_USER (
	NAME varchar(50) not null,
	PASSWORD varchar(20) not null,
	ROLE varchar(15)) ENGINE=InnoDB;
-- SQLINES LICENSE FOR EVALUATION USE ONLY
insert into CM_SUPER_USER values('admin', '5A191710A4AA7099', 'cm_super_user');

create table CM_ROOT  (
   OID					BIGINT     not null,
   UPDATETIME			DATETIME		   default NOW(),
   constraint PK_CM_ROOT primary key (OID)) ENGINE=InnoDB;

create table MM_ATTRIBUTE (
	ID BIGINT not null,
	NAME VARCHAR(30) not null,
	DISPLAY_NAME VARCHAR(100) not null,
	CATEGORY TINYINT not null constraint MM_ATTRIBUTECATEGORY_CHK check (CATEGORY in (1,2)) ,
	DESCRIPTION VARCHAR(2000) null,
	COLUMN_NAME VARCHAR(30) null,
	CLASS_ID BIGINT not null, constraint MM_ATTRIBUTE_PK primary key (ID) ) ENGINE=InnoDB; 

create table MM_RELATION_ATTRIBUTE (
	ATTRIBUTE_ID BIGINT not null,
	TYPE TINYINT not null constraint MM_RELATION_ATTRIBUTETYPE_CHK check (TYPE in (1,2)) ,
	OWNERSHIP TINYINT not null constraint MM_RELATION_OWNERSHIP_CHK check (OWNERSHIP in (1,2,3)) ,
	IS_JOINTABLE TINYINT not null,
	JOINTABLE_NAME VARCHAR(30) null,
	IS_FK TINYINT not null,
	REF_CLASS_ID BIGINT not null, constraint MM_RELATION_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ) ENGINE=InnoDB; 

create table MM_SIMPLE_ATTRIBUTE (
	ATTRIBUTE_ID BIGINT not null,
	TYPE TINYINT not null,
	constraint MM_SIMPLE_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ) ENGINE=InnoDB; 

create table MM_PARENT_CHILD (
	PARENT_ID BIGINT not null,
	CHILD_ID BIGINT not null,
	constraint MM_PARENT_CHILD_PK primary key (PARENT_ID, CHILD_ID) ) ENGINE=InnoDB; 

create table MM_CLASS (
	ID BIGINT not null,
	NAME VARCHAR(30) not null,
	DISPLAY_NAME VARCHAR(100) not null,
	TABLE_NAME VARCHAR(30) null,
	SCHEMA_ID BIGINT not null,
	PK_NAME varchar(30),
	constraint MM_CLASS_PK primary key (ID) ) ENGINE=InnoDB; 

create table MM_SCHEMA (
	ID BIGINT not null,
	NAME VARCHAR(50) not null,
	VERSION VARCHAR(15) not null,
	MAP_METHOD TINYINT not null constraint MM_SCHEMAMAP_METHOD_CHK check (MAP_METHOD in (1,2,3)) ,
	XML_SCHEMA LONGTEXT,
	XACL LONGTEXT,
	QUERIES LONGTEXT,
	TAXONOMY LONGTEXT,
	RULES LONGTEXT,
	MAPPINGS LONGTEXT,
	LOG LONGTEXT,
	constraint MM_SCHEMA_PK primary key (ID) ) ENGINE=InnoDB; 

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

create table key_generate(key_value double primary key,
	class_id double not null,
	attribute_id double not null,
	schema_id double not null,	
	transformer_id double not null,
	constraint UN_KEY_CLASS unique(class_id)) ENGINE=InnoDB;

insert into key_generate values(0, 0, 1, 1, 1);

create table CM_ATTACHMENT  (
   ID					VARCHAR(100)  not null ,
   OID					BIGINT     not null,
   NAME                 VARCHAR(50)   not null,
   CLASSNAME            VARCHAR(30)   not null,
   TYPE                 VARCHAR(20)   not null,
   CREATETIME			DATETIME		   default NOW(),
   ASIZE				BIGINT,
   IS_PUBLIC			TINYINT    not null,
   ATTACHMENT			LONGBLOB,
   CONSTRAINT PK_CM_ATTACHMENT PRIMARY KEY (ID),
   CONSTRAINT CM_ATTACHMENT_UN_ID UNIQUE(OID, NAME),
   CONSTRAINT FK_ATTACHEMNT_ROOT FOREIGN KEY(OID)
		REFERENCES CM_ROOT(OID) ON DELETE CASCADE   
   ) ENGINE=InnoDB;