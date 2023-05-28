create table WF_WORKFLOW_INSTANCE (
	WorkflowInstanceId varchar(100) not null,
	WorkflowTypeId BIGINT not null,
	InitializedDateTime DATETIME,
	EndDateTime DATETIME,
	CurrentStatus varchar(20) not null,
	constraint WF_WORKFLOW_INSTANCE_PK primary key (WorkflowInstanceId)) ENGINE=InnoDB;

create table WF_ACTIVITY_INSTANCE (
	ActivityInstanceId varchar(100) not null,
	QualifiedName varchar(300) not null,
	InitializedDateTime DATETIME,
	EndDateTime DATETIME,
	CurrentStatus varchar(20) not null,
	WorkflowInstanceId varchar(100),
	constraint WF_ACTIVITY_INSTANCE_PK primary key (ActivityInstanceId),
   	constraint WF_WORKFLOW_INSTANCE_FK FOREIGN KEY(WorkflowInstanceId)
		REFERENCES WF_WORKFLOW_INSTANCE (WorkflowInstanceId) ON DELETE CASCADE) ENGINE=InnoDB;