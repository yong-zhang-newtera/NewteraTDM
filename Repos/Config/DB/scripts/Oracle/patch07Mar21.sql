create table WF_WFEVENT_SUBSCRIPTION (
	SubscriptionId varchar2(100) not null,
	ParentWFInstanceID varchar2(100) not null,
	ChildWFInstanceID varchar2(100) not null,
	QueueName varchar2(100) not null,
	EventType varchar2(20));