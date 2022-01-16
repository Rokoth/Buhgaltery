create extension if not exists "uuid-ossp";

create table if not exists settings(	 
	  id            int           not null primary key
	, param_name    varchar(100)  not null
	, param_value   varchar(1000) not null	
);

create table if not exists "user"(
      id            uuid          not null default uuid_generate_v4() primary key
	, "name"        varchar(100)  not null
	, "description" varchar(1000) null
	, "login"       varchar(100)  not null
	, "password"    bytea         not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists "h_user"(
      h_id          bigserial     not null primary key        
    , id            uuid          null
	, "name"        varchar(100)  null
	, "description" varchar(1000) null
	, "login"       varchar(100)  null
	, "password"    bytea         null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, change_date   timestamptz   not null default now()
	, "user_id"     varchar       null
);

create table if not exists product(
	  id            uuid          not null primary key
	, "name"        varchar(100)  not null
	, "description" varchar(1000) null
	, parent_id     uuid          null
	, add_period    int           not null
	, min_value     int           not null
	, max_value     int           not null
	, is_leaf       boolean       not null
	, last_add_date timestamptz   not null
	, userid        uuid          not null
	, version_date  timestamptz   not null
	, is_deleted    boolean       not null default false	
);

create table if not exists h_product(
	  h_id          bigserial     not null primary key        
    , id            uuid          null
	, "name"        varchar(100)  null
	, "description" varchar(1000) null
	, parent_id     uuid          null
	, add_period    int           null
	, min_value     int           null
	, max_value     int           null
	, is_leaf       boolean       null
	, last_add_date timestamptz   null
	, userid        uuid          null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, change_date   timestamptz   not null default now()
	, "user_id"     varchar       null
);

create table if not exists formula(
	  id            uuid          not null default uuid_generate_v4() primary key
	, "name"        varchar(100)  not null
	, "text"        varchar(1000) not null	
	, is_default    boolean       not null
	, userid        uuid          not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists h_formula(
      h_id          bigserial     not null primary key
	, id            uuid          null
	, "name"        varchar(100)  null
	, "text"        varchar(1000) null	
	, is_default    boolean       null
	, userid        uuid          null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, "user_id"     varchar       null
	, change_date   timestamptz   not null
);

create table if not exists incoming(
	  id            uuid          not null default uuid_generate_v4() primary key
	, income_date   timestamptz   not null default now()
	, "description" varchar(1000) not null	
	, "value"       decimal       not null
	, userid        uuid          not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists h_incoming(
      h_id          bigserial     not null primary key
	, id            uuid          null
	, income_date   timestamptz   null
	, "description" varchar(1000) null	
	, "value"       decimal       null
	, userid        uuid          null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, "user_id"     varchar       null
	, change_date   timestamptz   not null
);

create table if not exists outgoing(
	  id            uuid          not null default uuid_generate_v4() primary key
	, out_date      timestamptz   not null default now()
	, product_id    uuid          not null
	, "description" varchar(1000) null	
	, "value"       decimal       not null
	, userid        uuid          not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists h_outgoing(
      h_id          bigserial     not null primary key
	, id            uuid          null
	, out_date      timestamptz   not null default now()
	, product_id    uuid          not null
	, "description" varchar(1000) null	
	, "value"       decimal       not null
	, userid        uuid          not null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, "user_id"     varchar       null
	, change_date   timestamptz   not null
);

create table if not exists reserve(
	  id            uuid          not null default uuid_generate_v4() primary key	
	, product_id    uuid          not null	
	, "value"       decimal       not null
	, userid        uuid          not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists h_reserve(
      h_id          bigserial     not null primary key
	, id            uuid          null
	, product_id    uuid          not null	
	, "value"       decimal       not null
	, userid        uuid          not null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, "user_id"     varchar       null
	, change_date   timestamptz   not null
);