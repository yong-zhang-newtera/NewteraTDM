create table WF_PROJECT (
	ID BIGINT not null,
	NAME varchar(100) not null,
	DESCRIPTION varchar(300),
	XML LONGTEXT,
	XACL LONGTEXT,
	UPDATELOG LONGTEXT,
	constraint WF_PROJECT_PK primary key (ID)) ENGINE=InnoDB;

create table WF_WORKFLOW (
	ID BIGINT not null,
	NAME varchar(100) not null,
    TYPE varchar(20) not null,
	CLASS_NAME varchar(200) not null,
	DESCRIPTION varchar(300),
	XOML LONGTEXT,
	RULES LONGTEXT,
	LAYOUT LONGTEXT,
	CODE LONGTEXT,
	PROJECT_ID BIGINT not null,
	constraint WF_WORKFLOW_PK primary key (ID),
   	CONSTRAINT WF_WORKFLOW_PROJECT_FK FOREIGN KEY(PROJECT_ID)
		REFERENCES WF_PROJECT (ID) ON DELETE CASCADE) ENGINE=InnoDB;

alter table KEY_GENERATE add PROJECT_ID BIGINT;

alter table KEY_GENERATE add WORKFLOW_ID BIGINT;

update KEY_GENERATE set PROJECT_ID=1, WORKFLOW_ID=1;

alter table mm_schema add(events LONGTEXT);

create table WF_INSTANCE_STATE (
	InstanceID varchar(100) not null,
	State LONGBLOB,
	Unlocked TINYINT,
	Modified datetime not null,
	constraint WF_INSTANCE_STATE_PK primary key (InstanceID)) ENGINE=InnoDB;

create table WF_COMPLETED_SCOPE (
	CompletedScopeID varchar(100) not null,
	State LONGBLOB,
	Modified datetime not null) ENGINE=InnoDB;

create table WF_INSTANCE_MAP (
	DataInstanceId BIGINT,
	DataClassName varchar(50),
	SchemaId varchar(50),
	WFInstanceID varchar(100) not null,
	WorkflowTypeId BIGINT not null) ENGINE=InnoDB;

create table WF_EVENT_SUBSCRIPTION (
	SubscriptionId varchar(100) not null,
	WFInstanceID varchar(100) not null,
	QueueName varchar(100) not null,
	SchemaId varchar(100) not null,
	ClassName varchar(150) not null,
	EventName varchar(150) not null,
	CreateBinding varchar(10) not null) ENGINE=InnoDB;