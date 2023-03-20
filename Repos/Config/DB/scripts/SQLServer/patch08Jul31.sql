alter table WF_PROJECT add VERSION nvarchar(20) null;
update WF_PROJECT set VERSION = '1.0';
