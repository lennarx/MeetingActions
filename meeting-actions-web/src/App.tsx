import { BrowserRouter, Routes, Route } from 'react-router-dom';
import CreateJob from './pages/CreateJob';
import Processing from './pages/Processing';
import Result from './pages/Result';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<CreateJob />} />
        <Route path="/processing" element={<Processing />} />
        <Route path="/result" element={<Result />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
