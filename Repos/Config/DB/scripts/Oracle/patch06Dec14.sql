create table WF_PROJECT (
	ID NUMBER(38,0) not null,
	NAME varchar2(100) not null,
	DESCRIPTION varchar2(300),
	XML CLOB default empty_clob(),
	XACL CLOB default empty_clob(),
	UPDATELOG CLOB default empty_clob(),
	constraint WF_PROJECT_PK primary key (ID));

create table WF_WORKFLOW (
	ID NUMBER(38,0) not null,
	NAME varchar2(100) not null,
        TYPE varchar2(20) not null,
	CLASS_NAME varchar2(200) not null,
	DESCRIPTION varchar2(300),
	XOML CLOB default empty_clob(),
	RULES CLOB default empty_clob(),
	LAYOUT CLOB default empty_clob(),
	CODE CLOB default empty_clob(),
	PROJECT_ID NUMBER(38,0) not null,
	constraint WF_WORKFLOW_PK primary key (ID),
   	CONSTRAINT WF_WORKFLOW_PROJECT_FK FOREIGN KEY(PROJECT_ID)
		REFERENCES WF_PROJECT (ID) ON DELETE CASCADE);

alter table KEY_GENERATE add PROJECT_ID number;

alter table KEY_GENERATE add WORKFLOW_ID number;

update KEY_GENERATE set PROJECT_ID=1, WORKFLOW_ID=1;

alter table mm_schema add(events clob default empty_clob());

create table WF_INSTANCE_STATE (
	InstanceID varchar2(100) not null,
	State BLOB,
	Unlocked NUMBER(1,0),
	Modified date not null,
	constraint WF_INSTANCE_STATE_PK primary key (InstanceID));

create table WF_COMPLETED_SCOPE (
	CompletedScopeID varchar2(100) not null,
	State BLOB,
	Modified date not null);

create table WF_INSTANCE_MAP (
	DataInstanceId NUMBER(38,0),
	DataClassName varchar2(50),
	SchemaId varchar2(50),
	WFInstanceID varchar2(100) not null,
	WorkflowTypeId NUMBER(38,0) not null);

create table WF_EVENT_SUBSCRIPTION (
	SubscriptionId varchar2(100) not null,
	WFInstanceID varchar2(100) not null,
	QueueName varchar2(100) not null,
	SchemaId varchar2(100) not null,
	ClassName varchar2(150) not null,
	EventName varchar2(150) not null,
	CreateBinding varchar2(10) not null);