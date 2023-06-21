# Stable Diffusion Unity

An AI exhibit showcasing human-machine collaboration using stable diffusion and unity to generate skyboxes.

## Overview

This project was created by the Emergent Media Center's 2023 Creative AI Sandbox Team.  The purpose is to explore the applications of AI and create an interactive exhibit to display the possibilities.  One such possibility explored here is a skybox generator that accepts a user's text prompt to generate a skybox in Unity.

## Process

The following steps are used to achieve this:
* Automatic 1111 must be run with the --api command line argument
* Accept text from a user in Unity
* Make an API call to Automatic 1111 to generate an image
* Inpaint the edges with tiling enabled using a mask so the image can repeat
* Disable mipmaps on the resulting image to eliminate seams
* Pass the image to the material being used for the Unity skybox to display it

## Team

* Rachel Hooper
* Alexandre Tolstenko
* Dimitri Sophinos
* Ananda Shumock-Bailey
* Dillon Drummond
* Alissa Kokorovic

## Progress

- [ ] Automatically Download and Run Stable Diffusion If Not Already Installed
- [X] Stable Diffusion Image Generation from Unity Text Prompt
	- [ ] Stable Diffusion Image Generation from Drawing Prompt
- [X] Img2Img Inpainting for Tiling
- [X] Generated Image Displayed on Unity Skybox
- [X] VR Drawing
- [ ] Drawing Prompts on Skyboxes
- [ ] VR Integrated Into Skybox Generation
- [X] UI Designs
- [ ] UI Implementation