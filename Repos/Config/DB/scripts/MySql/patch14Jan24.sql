create table WF_DB_EVENT_CONTEXT(
	ContextId varchar(100) not null,
	SchemaId varchar(50) not null,
	ClassName varchar(100) not null,
	InstanceId varchar(100) not null,
	OperationType varchar(50) not null,
	Attributes varchar(500)) ENGINE=InnoDB;