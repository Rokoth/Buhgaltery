insert into formula( id , "name" , "text" , is_default, version_date, is_deleted)
values (uuid_generate_v4(), 'default_formula', '', true, now(), false);

insert into "user"(id, name, description, login, password, formula_id, version_date, is_deleted)
values (uuid_generate_v4(), 'admin', 'admin', 'admin', sha512('admin'),  (select id from formula limit 1), now(), false);

insert into user_settings(id, userid, default_reserve_value,leaf_only, version_date, is_deleted)
values(uuid_generate_v4(), (select id from "user" limit 1), 100, true, now(), false);