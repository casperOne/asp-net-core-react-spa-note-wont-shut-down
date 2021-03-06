import ReactDOM from 'react-dom';

// Service worker.
import { unregister } from './serviceWorker';

// Get the base URL and the root element where
// the app is hosted.
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

// If there is no base, throw.
if (!baseUrl) throw new Error("Could not define the URL of the document from the 'href' attribute on the 'base' element.")

// Render.
ReactDOM.render(<div>Hello there</div>, rootElement);

// Register service worker.
unregister();
