/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "**/*.cshtml"
  ],
  theme: {
    extend: {},
  },
  plugins: [
    require("daisyui"),
  ],
  daisyui: {
    themes: ["corporate"]
  }
}

