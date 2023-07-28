--Copyright 2021 Dmitriy Rokoth
--Licensed under the Apache License, Version 2.0
--
--ref1

create table settings(	 
	  id            int           not null primary key
	, param_name    varchar(100)  not null
	, param_value   varchar(1000) not null	
);