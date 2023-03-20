create table CM_CLS_ATTACHMENT  (
   ID					nvarchar(100)  not null ,
   CID					bigint  	not null,
   SCHEMA_ID 				bigint,
   NAME                 nvarchar(50)   not null,
   CLASSNAME            nvarchar(30)   not null,
   TYPE                 nvarchar(20)   not null,
   CREATETIME			datetime	default getdate(),
   ASIZE				int,
   IS_PUBLIC			bit				not null,
   ATTACHMENT			image,
   CONSTRAINT PK_CM_CLS_ATTACHMENT PRIMARY KEY (ID),
   CONSTRAINT CM__CLS_ATTACHMENT_UN_ID UNIQUE(CID, NAME),
   CONSTRAINT FK_ATTACHEMNT_CLASS FOREIGN KEY(CID)
		REFERENCES MM_CLASS(ID) ON DELETE CASCADE   
   );