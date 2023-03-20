create table WF_WORKFLOW_INSTANCE (
	WorkflowInstanceId varchar2(100) not null,
	WorkflowTypeId NUMBER(38,0) not null,
	InitializedDateTime DATE,
	EndDateTime DATE,
	CurrentStatus varchar2(20) not null,
	constraint WF_WORKFLOW_INSTANCE_PK primary key (WorkflowInstanceId));

create table WF_ACTIVITY_INSTANCE (
	ActivityInstanceId varchar2(100) not null,
	QualifiedName varchar2(300) not null,
	InitializedDateTime DATE,
	EndDateTime DATE,
	CurrentStatus varchar2(20) not null,
	WorkflowInstanceId varchar2(100),
	constraint WF_ACTIVITY_INSTANCE_PK primary key (ActivityInstanceId),
   	constraint WF_WORKFLOW_INSTANCE_FK FOREIGN KEY(WorkflowInstanceId)
		REFERENCES WF_WORKFLOW_INSTANCE (WorkflowInstanceId) ON DELETE CASCADE);