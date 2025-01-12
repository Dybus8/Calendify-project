import React from 'react';
import { Link } from 'react-router-dom';
import './Home.css'; // Importing the CSS file for styling

export default function Home() {
  return (
    <div className="container px-5 my-5 text-center">
      <h1>Calendify</h1>
      <h2>B.I.R. Bal Is Rond</h2>
      <div className="mt-4">
        <Link to="/login" className="btn btn-primary">Login</Link>
      </div>
    </div>
  );
}
