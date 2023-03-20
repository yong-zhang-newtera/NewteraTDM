create table CM_PATCH (
	patch_name nvarchar(50) not null primary key);

create table CM_SUPER_USER (
	NAME nvarchar(50) not null,
	PASSWORD nvarchar(20) not null,
	ROLE nvarchar(15));

insert into CM_SUPER_USER values('admin', '5A191710A4AA7099', 'cm_super_user');

create table CM_ROOT  (
   OID				bigint     not null,
   UPDATETIME		smalldatetime		   default getdate(),
   constraint PK_CM_ROOT primary key (OID));

create table MM_ATTRIBUTE (
	ID bigint not null,
	NAME nvarchar(30) not null,
	DISPLAY_NAME nvarchar(100) not null,
	CATEGORY int not null constraint MM_ATTRIBUTECATEGORY_CHK check (CATEGORY in (1,2)) ,
	DESCRIPTION nvarchar(2000) null,
	COLUMN_NAME nvarchar(30) null,
	CLASS_ID bigint not null, constraint MM_ATTRIBUTE_PK primary key (ID) ); 

create table MM_RELATION_ATTRIBUTE (
	ATTRIBUTE_ID bigint not null,
	TYPE int not null constraint MM_RELATION_ATTRIBUTETYPE_CHK check (TYPE in (1,2)) ,
	OWNERSHIP int not null constraint MM_RELATION_OWNERSHIP_CHK check (OWNERSHIP in (1,2,3)) ,
	IS_JOINTABLE int not null,
	JOINTABLE_NAME nvarchar(30) null,
	IS_FK int not null,
	REF_CLASS_ID bigint not null, constraint MM_RELATION_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ); 


create table MM_SIMPLE_ATTRIBUTE (
	ATTRIBUTE_ID bigint not null,
	TYPE int not null,
	constraint MM_SIMPLE_ATTRIBUTE_PK primary key (ATTRIBUTE_ID) ); 

create table MM_PARENT_CHILD (
	PARENT_ID bigint not null,
	CHILD_ID bigint not null,
	constraint MM_PARENT_CHILD_PK primary key (PARENT_ID, CHILD_ID) ); 

create table MM_CLASS (
	ID bigint not null,
	NAME nvarchar(30) not null,
	DISPLAY_NAME nvarchar(100) not null,
	TABLE_NAME nvarchar(30) null,
	SCHEMA_ID bigint not null,
	PK_NAME nvarchar(30),
	constraint MM_CLASS_PK primary key (ID) ); 

create table MM_SCHEMA (
	ID bigint not null,
	NAME nvarchar(50) not null,
	DESCRIPTION nvarchar(200),
	VERSION nvarchar(15) not null,
	MAP_METHOD int not null constraint MM_SCHEMAMAP_METHOD_CHK check (MAP_METHOD in (1,2,3)) ,
	XML_SCHEMA text,
	XACL text ,
	QUERIES text,
	TAXONOMY text,
	RULES text,
	MAPPINGS text,
	LOG text,
	constraint MM_SCHEMA_PK primary key (ID) ); 

alter table MM_ATTRIBUTE add constraint MM_ATTRIBUTE_UC1 unique (
	CLASS_ID,
	NAME); 

alter table MM_SCHEMA add constraint MM_SCHEMA_UC1 unique (
	NAME,
	VERSION); 

alter table MM_RELATION_ATTRIBUTE
	add constraint MM_RELATION_ATTRIBUTE_FK1 foreign key (ATTRIBUTE_ID)
    references MM_ATTRIBUTE (ID) ON DELETE CASCADE; 

alter table MM_RELATION_ATTRIBUTE add constraint MM_RELATION_ATTRIBUTE_REF_FK1 foreign key (REF_CLASS_ID)
	references MM_CLASS (ID) ON DELETE CASCADE; 

alter table MM_SIMPLE_ATTRIBUTE add constraint MM_SIMPLE_ATTRIBUTE_FK1 foreign key (ATTRIBUTE_ID)
	references MM_ATTRIBUTE (ID) ON DELETE CASCADE;

alter table MM_PARENT_CHILD add constraint MM_PARENT_CHILD_CLASS_FK1 foreign key (PARENT_ID)
	 references MM_CLASS (ID) ON DELETE CASCADE;

alter table MM_PARENT_CHILD add constraint MM_PARENT_CHILD_CLASS_FK2 foreign key (CHILD_ID)
	 references MM_CLASS (ID) ON DELETE NO ACTION; 

alter table MM_CLASS add constraint MM_CLASS_SCHEMA_FK1 foreign key (SCHEMA_ID)
	 references MM_SCHEMA (ID) ON DELETE CASCADE;

create table KEY_GENERATE(KEY_VALUE bigint primary key,
	CLASS_ID bigint not null,
	ATTRIBUTE_ID bigint not null,
	SCHEMA_ID bigint not null,
	TRANSFORMER_ID bigint not null,	
	constraint UNI_KEY_CLASS unique(CLASS_ID));
insert into KEY_GENERATE values(0, 0, 1, 1, 1);

create table CM_ATTACHMENT  (
   ID					nvarchar(100)  not null ,
   OID					bigint  	not null,
   NAME                 nvarchar(50)   not null,
   CLASSNAME            nvarchar(30)   not null,
   TYPE                 nvarchar(20)   not null,
   CREATETIME			datetime	default getdate(),
   ASIZE				int,
   IS_PUBLIC			bit				not null,
   ATTACHMENT			image,
   CONSTRAINT PK_CM_ATTACHMENT PRIMARY KEY (ID),
   CONSTRAINT CM_ATTACHMENT_UN_ID UNIQUE(OID, NAME),
   CONSTRAINT FK_ATTACHEMNT_ROOT FOREIGN KEY(OID)
		REFERENCES CM_ROOT(OID) ON DELETE CASCADE   
   );