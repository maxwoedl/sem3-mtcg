--
-- PostgreSQL database dump
--

-- Dumped from database version 14.1
-- Dumped by pg_dump version 14.1

-- Started on 2022-01-10 20:08:33 CET

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3596 (class 1262 OID 16394)
-- Name: mtcg; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE mtcg WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'en_GB.UTF-8';


ALTER DATABASE mtcg OWNER TO postgres;

\connect mtcg

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3597 (class 0 OID 0)
-- Name: mtcg; Type: DATABASE PROPERTIES; Schema: -; Owner: postgres
--

ALTER DATABASE mtcg CONNECTION LIMIT = 100;


\connect mtcg

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 210 (class 1259 OID 16408)
-- Name: cards; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cards (
    id uuid NOT NULL,
    name character varying(50) NOT NULL,
    owner character varying(40),
    cardtype character varying(20) NOT NULL,
    elementtype character varying(20) NOT NULL,
    damage double precision DEFAULT 0 NOT NULL,
    deck boolean DEFAULT false NOT NULL,
    shiny boolean DEFAULT false NOT NULL
);


ALTER TABLE public.cards OWNER TO postgres;

--
-- TOC entry 211 (class 1259 OID 16421)
-- Name: packages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.packages (
    id uuid NOT NULL,
    price integer DEFAULT 5 NOT NULL,
    cards uuid[] NOT NULL,
    owner character varying(40)
);


ALTER TABLE public.packages OWNER TO postgres;

--
-- TOC entry 209 (class 1259 OID 16400)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    username character varying(40) NOT NULL,
    password character varying(40) NOT NULL,
    name character varying(50),
    bio character varying(250) DEFAULT NULL::character varying,
    image character varying(100),
    coins integer DEFAULT 20 NOT NULL,
    elo integer DEFAULT 100 NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 3447 (class 2606 OID 16415)
-- Name: cards cards_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT cards_pkey PRIMARY KEY (id);


--
-- TOC entry 3449 (class 2606 OID 16428)
-- Name: packages packages_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.packages
    ADD CONSTRAINT packages_pkey PRIMARY KEY (id);


--
-- TOC entry 3445 (class 2606 OID 16404)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (username);


--
-- TOC entry 3451 (class 2606 OID 16429)
-- Name: packages fk_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.packages
    ADD CONSTRAINT fk_owner FOREIGN KEY (owner) REFERENCES public.users(username) NOT VALID;


--
-- TOC entry 3450 (class 2606 OID 16416)
-- Name: cards fk_username; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT fk_username FOREIGN KEY (owner) REFERENCES public.users(username) ON DELETE SET NULL NOT VALID;


-- Completed on 2022-01-10 20:08:33 CET

--
-- PostgreSQL database dump complete
--

