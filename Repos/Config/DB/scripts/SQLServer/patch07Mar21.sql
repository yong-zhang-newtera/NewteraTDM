create table WF_WFEVENT_SUBSCRIPTION (
	SubscriptionId nvarchar(100) not null,
	ParentWFInstanceID nvarchar(100) not null,
	ChildWFInstanceID nvarchar(100) not null,
	QueueName nvarchar(100) not null,
	EventType nvarchar(20));