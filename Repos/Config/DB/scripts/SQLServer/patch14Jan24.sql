create table WF_DB_EVENT_CONTEXT(
	ContextId nvarchar(100) not null,
	SchemaId nvarchar(50) not null,
	ClassName nvarchar(100) not null,
	InstanceId nvarchar(100) not null,
	OperationType nvarchar(50) not null,
	Attributes nvarchar(500));

