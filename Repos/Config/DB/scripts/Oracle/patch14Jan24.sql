create table WF_DB_EVENT_CONTEXT(
	ContextId varchar2(100) not null,
	SchemaId varchar2(50) not null,
	ClassName varchar2(100) not null,
	InstanceId varchar2(100) not null,
	OperationType varchar2(50) not null,
	Attributes varchar2(500));

