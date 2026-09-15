import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  getHomeData,
  submitConsultation,
  getHomeErrorMessage,
} from '../../services/homeService';