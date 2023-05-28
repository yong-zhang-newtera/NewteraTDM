alter table WF_PROJECT add(VERSION VARCHAR(20) null);
update WF_PROJECT set VERSION = '1.0';