/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["**/*.cshtml", "**/*.razor"],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
  daisyui: {
    themes: ["emerald"],
  },
};
