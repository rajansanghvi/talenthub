create table if not exists app_users
(
  id int(11) AUTO_INCREMENT primary key
  , email_id varchar(255) not null
  , username varchar(255) not null
  , hashed_password varchar(255) not null
  , device_id varchar(50) not null
  , auth_key varchar(50) not null
  , deleted tinyint(4) default 0
  , created_by varchar(255) not null
  , created_date datetime default now()
  , modified_by varchar(255) null
  , modified_date datetime null
)engine innoDb;

drop procedure if exists addIndex;
CREATE PROCEDURE addIndex() 
BEGIN
  if not exists(select 1 from information_schema.statistics where TABLE_NAME = 'app_users' and index_NAME = 'idx_app_users_email_id') then
    create index idx_app_users_email_id on app_users(email_id);
  end if;
END;
call addIndex;

drop procedure if exists addIndex;
CREATE PROCEDURE addIndex() 
BEGIN
  if not exists(select 1 from information_schema.statistics where TABLE_NAME = 'app_users' and index_NAME = 'idx_app_users_username') then
    create index idx_app_users_username on app_users(username);
  end if;
END;
call addIndex;

drop procedure if exists addIndex;
CREATE PROCEDURE addIndex() 
BEGIN
  if not exists(select 1 from information_schema.statistics where TABLE_NAME = 'app_users' and index_NAME = 'idx_app_users_device_id') then
    create index idx_app_users_device_id on app_users(device_id);
  end if;
END;
call addIndex;