create table WF_REASSIGNED_TASK (
        TaskId bigint not null,
	OriginalOwner nvarchar(100) not null,
	CurrentOwner nvarchar(100) not null,
	WorkflowInstanceId nvarchar(100));