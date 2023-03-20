create table WF_PROJECT (
	ID bigint not null,
	NAME nvarchar(100) not null,
	DESCRIPTION nvarchar(300),
	XML text,
	XACL text,
	UPDATELOG text,
	constraint WF_PROJECT_PK primary key (ID));

create table WF_WORKFLOW (
	ID bigint not null,
	NAME nvarchar(100) not null,
        TYPE varchar(20) not null,
	CLASS_NAME nvarchar(200) not null,
	DESCRIPTION nvarchar(300),
	XOML text,
	RULES text ,
	LAYOUT text,
	CODE text,
	PROJECT_ID bigint not null,
	constraint WF_WORKFLOW_PK primary key (ID),
   	CONSTRAINT WF_WORKFLOW_PROJECT_FK FOREIGN KEY(PROJECT_ID)
		REFERENCES WF_PROJECT (ID) ON DELETE CASCADE);

alter table KEY_GENERATE add PROJECT_ID bigint;

alter table KEY_GENERATE add WORKFLOW_ID bigint;

update KEY_GENERATE set PROJECT_ID=1, WORKFLOW_ID=1;

alter table mm_schema add events text null;

create table WF_INSTANCE_STATE (
	InstanceID nvarchar(100) not null,
	State image null,
	Unlocked int null,
	Modified datetime not null,
	constraint WF_INSTANCE_STATE_PK primary key (InstanceID));

create table WF_COMPLETED_SCOPE (
	CompletedScopeID nvarchar(100) not null,
	State image NULL,
	Modified datetime not null);

create table WF_INSTANCE_MAP (
	DataInstanceId bigint,
	DataClassName nvarchar(50),
	SchemaId nvarchar(50),
	WFInstanceID nvarchar(100) not null,
	WorkflowTypeId bigint not null);

create table WF_EVENT_SUBSCRIPTION (
	SubscriptionId nvarchar(100) not null,
	WFInstanceID nvarchar(100) not null,
	QueueName nvarchar(100) not null,
	SchemaId nvarchar(100) not null,
	ClassName nvarchar(150) not null,
	EventName nvarchar(150) not null,
	CreateBinding nvarchar(10) not null);

