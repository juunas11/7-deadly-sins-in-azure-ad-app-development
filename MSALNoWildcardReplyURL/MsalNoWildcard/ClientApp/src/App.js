import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Login from './components/Login';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import AuthenticatedRoute from './components/AuthenticatedRoute';
import AadCallback from './components/AadCallback';

export default () => (
  <Layout>
    <Route exact path='/aad-callback' component={AadCallback} />
    <Route exact path='/' component={Home} />
    <Route exact path='/login' component={Login} />
    <AuthenticatedRoute path='/counter' component={Counter} />
    <AuthenticatedRoute path='/fetch-data/:startDateIndex?' component={FetchData} />
  </Layout>
);
