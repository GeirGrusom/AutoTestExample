create table product
(
	id uuid not null,
	name text not null,
	description text not null,
	price decimal not null,

	constraint pk_products primary key(id),
	constraint chk_products_id check(id != '00000000-0000-0000-0000-000000000000'),
	constraint chk_products_name check(length(name) > 1),
	constraint chk_products_price_not_negative check(price >= 0)
);

create table shopping_cart
(
	id uuid not null,

	constraint pk_shopping_cart primary key(id),
	constraint shopping_cart_id_not_default check(id != '00000000-0000-0000-0000-000000000000')
);

create table shopping_cart_line
(
	id int not null,
	shopping_cart_id uuid not null,
	product_id uuid not null,
	amount int not null,

	constraint pk_shopping_cart_line primary key(shopping_cart_id, id),
	constraint fk_shopping_cart_line_shopping_cart foreign key(shopping_cart_id) references shopping_cart(id),
	constraint fk_shopping_cart_line_product foreign key(product_id) references product(id),
	constraint chk_shopping_cart_line_positive_amounts check(amount > 0)
);

create index idx_shopping_cart_line_shoppint_cart on shopping_cart_line(shopping_cart_id);