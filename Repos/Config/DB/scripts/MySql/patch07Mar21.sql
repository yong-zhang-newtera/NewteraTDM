create table WF_WFEVENT_SUBSCRIPTION (
	SubscriptionId varchar(100) not null,
	ParentWFInstanceID varchar(100) not null,
	ChildWFInstanceID varchar(100) not null,
	QueueName varchar(100) not null,
	EventType varchar(20)) ENGINE=InnoDB;