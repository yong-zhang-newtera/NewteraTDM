create table WF_REASSIGNED_TASK (
        TaskId NUMBER(38,0) not null,
	OriginalOwner varchar2(100) not null,
	CurrentOwner varchar2(100) not null,
	WorkflowInstanceId varchar2(100));