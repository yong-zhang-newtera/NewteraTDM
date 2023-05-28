create table WF_REASSIGNED_TASK (
    TaskId BIGINT not null,
	OriginalOwner varchar(100) not null,
	CurrentOwner varchar(100) not null,
	WorkflowInstanceId varchar(100)) ENGINE=InnoDB;