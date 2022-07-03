use Dalilak_DB;

create table Admin 
( 
	id varchar(32) primary key not null ,
	email varchar(255) unique not null,
	password varchar(255) not null
 );
 
 create table Users (
	 id varchar(32) primary key not null ,
	 phone_num char(13) unique not null,
	 email varchar(255) unique not null,
	 name varchar(255) not null,
	 user_type tinyint(1) not null,
	 age int null,
	 image blob null ,
	 information text null,
	 record_doc varchar(32) null,
	 city_id varchar(32) null
 );
 
CREATE TABLE Places (
	id varchar(32) primary key not null,
    name varchar(255) not null,
    location varchar(255) not null,
    description text null,
    place_type char(3) not null,
    crowd_status varchar(50) null,
    related_doc varchar(32) null,
    statstc_doc varchar(32) null,
    totl_likes int default 0,
    totl_visits int default 0,
    city_id varchar(32)
);

CREATE TABLE Cities(
	id varchar(32) primary key not null,
    name varchar(255) not null,
    location varchar(255) not null
);
alter table Users
add constraint FK_City_ToUsers foreign key (city_id) references Cities(id);

alter table Places
add constraint FK_City_ToPlace foreign key (city_id) references Cities(id);

CREATE TABLE Schedules(
	Doc_id varchar(32) not null,
    user_id varchar(32) not null,
    constraint FK_User_Trips foreign key (user_id) references Users(id)
);

CREATE TABLE Requests(
	admin_id varchar(32) not null,
    user_id varchar(32) not null,
    file blob not null,
    req_status tinyint default 0,
    constraint FK_admin_Response foreign key (admin_id) references Admin(id),
    constraint FK_user_Ask foreign key (user_id) references Users(id)
);

CREATE TABLE Modifications(
	admin_id varchar(32) not null,
    user_id varchar(32) not null,
    operation varchar(255) not null,
    constraint FK_admin_Confirm foreign key (admin_id) references Admin(id),
    constraint FK_user_Modify foreign key (user_id) references Users(id)
);


create table Ads 
( 
	admin_id varchar(32),
	city_id varchar(32),
	ad_image blob  
 );
 alter table Ads
add constraint FK_City_ToControl foreign key (city_id) references Cities(id);

 alter table Ads
add constraint FK_Admin_Control foreign key (admin_id) references Admin(id);

