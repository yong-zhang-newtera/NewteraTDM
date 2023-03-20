create table WF_WORKFLOW_INSTANCE (
	WorkflowInstanceId nvarchar(100) not null,
	WorkflowTypeId bigint not null,
	InitializedDateTime datetime,
	EndDateTime datetime,
	CurrentStatus nvarchar(20) not null,
	constraint WF_WORKFLOW_INSTANCE_PK primary key (WorkflowInstanceId));

create table WF_ACTIVITY_INSTANCE (
	ActivityInstanceId nvarchar(100) not null,
	QualifiedName nvarchar(300) not null,
	InitializedDateTime datetime,
	EndDateTime datetime,
	CurrentStatus nvarchar(20) not null,
	WorkflowInstanceId nvarchar(100),
	constraint WF_ACTIVITY_INSTANCE_PK primary key (ActivityInstanceId),
   	constraint WF_WORKFLOW_INSTANCE_FK FOREIGN KEY(WorkflowInstanceId)
		REFERENCES WF_WORKFLOW_INSTANCE (WorkflowInstanceId) ON DELETE CASCADE);